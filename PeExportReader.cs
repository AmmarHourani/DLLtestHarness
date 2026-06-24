using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DllTestHarness
{
    /// <summary>
    /// Reads the export table directly from a PE file on disk — no dumpbin,
    /// no DbgHelp dependency. Enumerates every exported function with its
    /// ordinal, RVA, and (where derivable) a parameter hint.
    ///
    /// IMPORTANT: a plain DLL's export table contains only names + addresses.
    /// True parameter *types* are not present in the binary. The only
    /// per-export argument info recoverable without a .pdb is the __stdcall
    /// x86 stack-byte decoration (_Name@N), which gives an argument byte count.
    /// </summary>
    internal static class PeExportReader
    {
        public class ExportEntry
        {
            public string Name;            // null for by-ordinal-only exports
            public uint Ordinal;
            public uint Rva;
            public bool IsForwarder;
            public string ForwarderTarget;
            public string ParamHint;       // best-effort, see class remarks
        }

        public static List<ExportEntry> Read(string path, out bool is64)
        {
            byte[] img = File.ReadAllBytes(path);
            is64 = false;

            if (img.Length < 0x40) throw new BadImageFormatException("File too small to be a PE.");
            if (img[0] != (byte)'M' || img[1] != (byte)'Z')
                throw new BadImageFormatException("Missing 'MZ' DOS signature.");

            int peOff = BitConverter.ToInt32(img, 0x3C);
            if (peOff < 0 || peOff + 24 > img.Length)
                throw new BadImageFormatException("Invalid PE header offset.");
            if (BitConverter.ToUInt32(img, peOff) != 0x00004550)
                throw new BadImageFormatException("Missing 'PE\\0\\0' signature.");

            int coff = peOff + 4;
            ushort numSections = BitConverter.ToUInt16(img, coff + 2);
            ushort optSize = BitConverter.ToUInt16(img, coff + 16);
            int opt = coff + 20;

            ushort magic = BitConverter.ToUInt16(img, opt);
            is64 = (magic == 0x20B);   // 0x10B = PE32, 0x20B = PE32+
            bool localIs64 = is64;

            // Export directory = data directory index 0.
            // Data directories begin at offset 96 (PE32) or 112 (PE32+) into the optional header.
            int dataDirOff = opt + (localIs64 ? 112 : 96);
            if (dataDirOff + 8 > img.Length)
                throw new BadImageFormatException("Optional header truncated.");

            uint expRva = BitConverter.ToUInt32(img, dataDirOff);
            uint expSize = BitConverter.ToUInt32(img, dataDirOff + 4);

            var result = new List<ExportEntry>();
            if (expRva == 0) return result;   // no export table at all

            // Section table follows the optional header.
            int secTableOff = opt + optSize;
            var secVa = new uint[numSections];
            var secVSize = new uint[numSections];
            var secRaw = new uint[numSections];
            var secRawSize = new uint[numSections];
            for (int i = 0; i < numSections; i++)
            {
                int s = secTableOff + i * 40;
                if (s + 40 > img.Length) throw new BadImageFormatException("Section table truncated.");
                secVSize[i]   = BitConverter.ToUInt32(img, s + 8);
                secVa[i]      = BitConverter.ToUInt32(img, s + 12);
                secRawSize[i] = BitConverter.ToUInt32(img, s + 16);
                secRaw[i]     = BitConverter.ToUInt32(img, s + 20);
            }

            int sectionCount = numSections;
            // Local RVA->file-offset translation over the captured section arrays.
            Func<uint, int> rvaToOff = delegate (uint rva)
            {
                for (int i = 0; i < sectionCount; i++)
                {
                    uint span = Math.Max(secVSize[i], secRawSize[i]);
                    if (rva >= secVa[i] && rva < secVa[i] + span)
                        return (int)(secRaw[i] + (rva - secVa[i]));
                }
                throw new BadImageFormatException("RVA 0x" + rva.ToString("X") + " not within any section.");
            };

            int ed = rvaToOff(expRva);
            uint ordinalBase  = BitConverter.ToUInt32(img, ed + 16);
            uint numFuncs     = BitConverter.ToUInt32(img, ed + 20);
            uint numNames     = BitConverter.ToUInt32(img, ed + 24);
            uint addrFuncsRva = BitConverter.ToUInt32(img, ed + 28);
            uint addrNamesRva = BitConverter.ToUInt32(img, ed + 32);
            uint addrOrdsRva  = BitConverter.ToUInt32(img, ed + 36);

            int funcs = rvaToOff(addrFuncsRva);
            int names = numNames > 0 ? rvaToOff(addrNamesRva) : 0;
            int ords  = numNames > 0 ? rvaToOff(addrOrdsRva) : 0;

            // Build index->name map (only some functions are exported by name).
            var nameByIndex = new Dictionary<int, string>();
            for (int i = 0; i < numNames; i++)
            {
                uint nameRva = BitConverter.ToUInt32(img, names + i * 4);
                ushort idx = BitConverter.ToUInt16(img, ords + i * 2);
                nameByIndex[idx] = ReadAsciiz(img, rvaToOff(nameRva));
            }

            for (int i = 0; i < numFuncs; i++)
            {
                uint funcRva = BitConverter.ToUInt32(img, funcs + i * 4);
                if (funcRva == 0) continue;   // unused slot

                var e = new ExportEntry
                {
                    Ordinal = ordinalBase + (uint)i,
                    Rva = funcRva,
                    Name = nameByIndex.ContainsKey(i) ? nameByIndex[i] : null
                };

                // Forwarder export: RVA points back into the export directory region.
                if (funcRva >= expRva && funcRva < expRva + expSize)
                {
                    e.IsForwarder = true;
                    e.ForwarderTarget = ReadAsciiz(img, rvaToOff(funcRva));
                }

                e.ParamHint = DeriveParamHint(e.Name, localIs64);
                result.Add(e);
            }

            // Sort by ordinal for stable display.
            result.Sort((a, b) => a.Ordinal.CompareTo(b.Ordinal));
            return result;
        }

        private static string ReadAsciiz(byte[] img, int off)
        {
            int end = off;
            while (end < img.Length && img[end] != 0) end++;
            return Encoding.ASCII.GetString(img, off, end - off);
        }

        /// <summary>
        /// Best-effort parameter description from the export name alone.
        /// Only __stdcall/__fastcall x86 decoration carries an arg byte count.
        /// </summary>
        public static string DeriveParamHint(string name, bool is64)
        {
            if (string.IsNullOrEmpty(name))
                return "by ordinal (no name — no signature info)";

            // MSVC C++ mangled names begin with '?'
            if (name.StartsWith("?"))
                return "C++ mangled — demangle for full signature";

            // __stdcall x86: optional leading '_', trailing @<bytes>
            Match m = Regex.Match(name, @"^_?[A-Za-z_][A-Za-z0-9_]*@(\d+)$");
            if (m.Success && !is64)
            {
                int bytes = int.Parse(m.Groups[1].Value);
                int approx = bytes / 4;
                return "__stdcall, " + bytes + " bytes args (~" + approx +
                       " 32-bit param" + (approx == 1 ? "" : "s") + ")";
            }

            // __fastcall x86: leading '@', trailing @<bytes>
            Match f = Regex.Match(name, @"^@[A-Za-z_][A-Za-z0-9_]*@(\d+)$");
            if (f.Success && !is64)
            {
                int bytes = int.Parse(f.Groups[1].Value);
                return "__fastcall, " + bytes + " bytes args";
            }

            return is64
                ? "x64 — types not stored in export table"
                : "undecorated (likely __cdecl — arg count unknown)";
        }
    }
}
