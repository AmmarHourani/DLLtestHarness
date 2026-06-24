using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DllTestHarness
{
    public partial class MainForm : Form
    {
        // ---- Win32 imports ----
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        // ---- State ----
        private IntPtr _hModule = IntPtr.Zero;
        private string _dllPath = null;

        // ---- Supported call signatures for the manual caller ----
        // Only true ABI types are offered. std::string / std::wstring are
        // deliberately absent: they are C++ template types with no stable
        // cross-compiler layout and cannot be marshalled across a raw pointer.
        // Export such functions through a C shim taking const char*/wchar_t*.
        private enum Signature
        {
            // void / int returning, no or simple int args
            VoidVoid,           // void f()
            IntVoid,            // int f()
            IntInt,             // int f(int)
            IntIntInt,          // int f(int, int)

            // 64-bit and unsigned integers
            Int64_Int64Int64,   // long long f(long long, long long)
            UIntVoid,           // unsigned int f()

            // floating point
            DoubleVoid,         // double f()
            DoubleDoubleDouble, // double f(double, double)
            FloatFloat,         // float f(float)

            // narrow strings (char*, ANSI)
            StrVoid,            // const char* f()
            IntStr,             // int f(const char*)
            StrStr,             // const char* f(const char*)

            // wide strings (wchar_t*, UTF-16 — Windows native)
            WStrVoid,           // const wchar_t* f()
            IntWStr,            // int f(const wchar_t*)
            WStrWStr,           // const wchar_t* f(const wchar_t*)
            IntWStrInt,         // int f(const wchar_t*, int)

            // BSTR (COM string — layout-stable, unlike std::wstring)
            BStrVoid,           // BSTR f()

            // pointers / handles
            VoidPtrVoid,        // void* f()
            IntPtrArg,          // int f(void*)

            // boolean
            BoolBool            // bool f(bool)   (marshalled as Win32 BOOL = int)
        }

        // --- Cdecl delegates ---
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void Fn_VoidVoid();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int Fn_IntVoid();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int Fn_IntInt(int a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int Fn_IntIntInt(int a, int b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate long Fn_Int64_Int64Int64(long a, long b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate uint Fn_UIntVoid();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate double Fn_DoubleVoid();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate double Fn_DoubleDoubleDouble(double a, double b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate float Fn_FloatFloat(float a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr Fn_StrVoid();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int Fn_IntStr([MarshalAs(UnmanagedType.LPStr)] string s);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr Fn_StrStr([MarshalAs(UnmanagedType.LPStr)] string s);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate IntPtr Fn_WStrVoid();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate int Fn_IntWStr([MarshalAs(UnmanagedType.LPWStr)] string s);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate IntPtr Fn_WStrWStr([MarshalAs(UnmanagedType.LPWStr)] string s);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private delegate int Fn_IntWStrInt([MarshalAs(UnmanagedType.LPWStr)] string s, int i);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.BStr)]
        private delegate string Fn_BStrVoid();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr Fn_VoidPtrVoid();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int Fn_IntPtrArg(IntPtr p);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private delegate bool Fn_BoolBool([MarshalAs(UnmanagedType.Bool)] bool b);

        // --- StdCall delegates (same set) ---
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void Fn_VoidVoid_S();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int Fn_IntVoid_S();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int Fn_IntInt_S(int a);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int Fn_IntIntInt_S(int a, int b);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate long Fn_Int64_Int64Int64_S(long a, long b);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate uint Fn_UIntVoid_S();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate double Fn_DoubleVoid_S();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate double Fn_DoubleDoubleDouble_S(double a, double b);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate float Fn_FloatFloat_S(float a);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr Fn_StrVoid_S();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int Fn_IntStr_S([MarshalAs(UnmanagedType.LPStr)] string s);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr Fn_StrStr_S([MarshalAs(UnmanagedType.LPStr)] string s);
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate IntPtr Fn_WStrVoid_S();
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate int Fn_IntWStr_S([MarshalAs(UnmanagedType.LPWStr)] string s);
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate IntPtr Fn_WStrWStr_S([MarshalAs(UnmanagedType.LPWStr)] string s);
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate int Fn_IntWStrInt_S([MarshalAs(UnmanagedType.LPWStr)] string s, int i);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        private delegate string Fn_BStrVoid_S();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate IntPtr Fn_VoidPtrVoid_S();
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int Fn_IntPtrArg_S(IntPtr p);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private delegate bool Fn_BoolBool_S([MarshalAs(UnmanagedType.Bool)] bool b);

        // .NET Framework 4.8 has no generic Marshal.GetDelegateForFunctionPointer<T>,
        // so wrap the non-generic overload.
        private static T GetDelegate<T>(IntPtr ptr) where T : class
        {
            return (T)(object)Marshal.GetDelegateForFunctionPointer(ptr, typeof(T));
        }


        public MainForm()
        {
            InitializeComponent();
            PopulateCombos();
            UpdateArgVisibility();
            UpdateModuleStatus();
        }

        private void PopulateCombos()
        {
            cmbConvention.Items.AddRange(new object[] { "Cdecl", "StdCall" });
            cmbConvention.SelectedIndex = 0;

            cmbSignature.Items.AddRange(new object[]
            {
                "void f()",
                "int f()",
                "int f(int)",
                "int f(int, int)",
                "long long f(long long, long long)",
                "unsigned int f()",
                "double f()",
                "double f(double, double)",
                "float f(float)",
                "const char* f()",
                "int f(const char*)",
                "const char* f(const char*)",
                "const wchar_t* f()",
                "int f(const wchar_t*)",
                "const wchar_t* f(const wchar_t*)",
                "int f(const wchar_t*, int)",
                "BSTR f()",
                "void* f()",
                "int f(void*)",
                "bool f(bool)"
            });
            cmbSignature.SelectedIndex = 0;
        }

        // ---- DLL lifecycle ----
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "DLL files (*.dll)|*.dll|All files (*.*)|*.*";
                ofd.Title = "Select a DLL";
                if (ofd.ShowDialog(this) == DialogResult.OK)
                    txtDllPath.Text = ofd.FileName;
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (_hModule != IntPtr.Zero)
            {
                Log("A module is already loaded. Unload it first.");
                return;
            }

            var path = txtDllPath.Text.Trim();
            if (string.IsNullOrEmpty(path))
            {
                Log("No DLL path specified.");
                return;
            }

            // List exports first (reads the file on disk; independent of LoadLibrary).
            ListExports(path);

            _hModule = LoadLibrary(path);
            if (_hModule == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                Log("LoadLibrary failed. Win32 error " + err + " (0x" + err.ToString("X8") + "): " +
                    new Win32Exception(err).Message);
                return;
            }

            _dllPath = path;
            Log("Loaded '" + path + "'");
            Log("  Module handle: 0x" + _hModule.ToInt64().ToString("X"));
            UpdateModuleStatus();
        }

        private void btnUnload_Click(object sender, EventArgs e)
        {
            if (_hModule == IntPtr.Zero)
            {
                Log("No module loaded.");
                return;
            }

            bool ok = FreeLibrary(_hModule);
            if (!ok)
            {
                int err = Marshal.GetLastWin32Error();
                Log("FreeLibrary failed. Win32 error " + err + " (0x" + err.ToString("X8") + "): " +
                    new Win32Exception(err).Message);
                return;
            }

            Log("Unloaded '" + _dllPath + "' (handle 0x" + _hModule.ToInt64().ToString("X") + ")");
            _hModule = IntPtr.Zero;
            _dllPath = null;
            UpdateModuleStatus();
        }

        // ---- Export enumeration ----
        private void ListExports(string path)
        {
            lstExports.Items.Clear();
            try
            {
                bool is64;
                var exports = PeExportReader.Read(path, out is64);
                Log("Parsed PE export table: " + exports.Count + " export(s), architecture " +
                    (is64 ? "x64" : "x86") + ".");

                if (is64 != Environment.Is64BitProcess)
                {
                    Log("  WARNING: DLL is " + (is64 ? "x64" : "x86") + " but this harness is " +
                        (Environment.Is64BitProcess ? "x64" : "x86") +
                        ". LoadLibrary will fail (error 193). Rebuild in the matching platform.");
                }

                foreach (var ex in exports)
                {
                    string display = ex.Name ?? ("(ordinal #" + ex.Ordinal + ")");
                    var item = new ListViewItem(display);
                    item.SubItems.Add(ex.Ordinal.ToString());
                    item.SubItems.Add("0x" + ex.Rva.ToString("X"));

                    string hint = ex.ParamHint;
                    if (ex.IsForwarder)
                        hint = "forwarder -> " + ex.ForwarderTarget;
                    item.SubItems.Add(hint);

                    // Stash the resolvable name (null for by-ordinal) for click-to-fill.
                    item.Tag = ex.Name;
                    lstExports.Items.Add(item);
                }

                if (exports.Count == 0)
                    Log("  (No exports found — the DLL may export nothing, or only resources.)");
            }
            catch (Exception ex)
            {
                Log("Could not parse exports: " + ex.GetType().Name + ": " + ex.Message);
            }
        }

        private void lstExports_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstExports.SelectedItems.Count == 0) return;
            var tag = lstExports.SelectedItems[0].Tag as string;
            if (!string.IsNullOrEmpty(tag))
                txtFuncName.Text = tag;   // click an export to load it into the caller
        }

        // ---- Resolve ----
        private IntPtr Resolve(string name, bool quiet = false)
        {
            if (_hModule == IntPtr.Zero)
            {
                if (!quiet) Log("No module loaded. Load a DLL first.");
                return IntPtr.Zero;
            }
            if (string.IsNullOrEmpty(name))
            {
                if (!quiet) Log("No function name specified.");
                return IntPtr.Zero;
            }

            IntPtr proc = GetProcAddress(_hModule, name);
            if (proc == IntPtr.Zero)
            {
                int err = Marshal.GetLastWin32Error();
                if (!quiet)
                    Log("GetProcAddress('" + name + "') failed. Win32 error " + err +
                        " (0x" + err.ToString("X8") + "): " + new Win32Exception(err).Message);
            }
            return proc;
        }

        private void btnProbe_Click(object sender, EventArgs e)
        {
            var name = txtFuncName.Text.Trim();
            IntPtr proc = Resolve(name);
            if (proc != IntPtr.Zero)
                Log("GetProcAddress('" + name + "') = 0x" + proc.ToInt64().ToString("X"));
        }

        // ---- Call ----
        private void btnCall_Click(object sender, EventArgs e)
        {
            var name = txtFuncName.Text.Trim();
            IntPtr proc = Resolve(name);
            if (proc == IntPtr.Zero) return;

            bool stdcall = cmbConvention.SelectedIndex == 1;
            var sig = (Signature)cmbSignature.SelectedIndex;

            try
            {
                Log("Calling '" + name + "' @ 0x" + proc.ToInt64().ToString("X") +
                    "  [" + (stdcall ? "StdCall" : "Cdecl") + "]  " + cmbSignature.SelectedItem);
                object result = Invoke(proc, sig, stdcall);
                if (result == null)
                    Log("  -> (void) returned");
                else
                    Log("  -> returned: " + result);
            }
            catch (Exception ex)
            {
                Log("  !! Exception during call: " + ex.GetType().Name + ": " + ex.Message);
                Log("     (Likely a signature/calling-convention mismatch or an access violation in the DLL.)");
            }
        }

        private object Invoke(IntPtr proc, Signature sig, bool stdcall)
        {
            switch (sig)
            {
                case Signature.VoidVoid:
                    if (stdcall) GetDelegate<Fn_VoidVoid_S>(proc)();
                    else GetDelegate<Fn_VoidVoid>(proc)();
                    return null;

                case Signature.IntVoid:
                    return stdcall
                        ? GetDelegate<Fn_IntVoid_S>(proc)()
                        : GetDelegate<Fn_IntVoid>(proc)();

                case Signature.IntInt:
                {
                    int a = ParseInt(txtArg1.Text, "Arg 1");
                    return stdcall
                        ? GetDelegate<Fn_IntInt_S>(proc)(a)
                        : GetDelegate<Fn_IntInt>(proc)(a);
                }

                case Signature.IntIntInt:
                {
                    int a = ParseInt(txtArg1.Text, "Arg 1");
                    int b = ParseInt(txtArg2.Text, "Arg 2");
                    return stdcall
                        ? GetDelegate<Fn_IntIntInt_S>(proc)(a, b)
                        : GetDelegate<Fn_IntIntInt>(proc)(a, b);
                }

                case Signature.Int64_Int64Int64:
                {
                    long a = ParseLong(txtArg1.Text, "Arg 1");
                    long b = ParseLong(txtArg2.Text, "Arg 2");
                    return stdcall
                        ? GetDelegate<Fn_Int64_Int64Int64_S>(proc)(a, b)
                        : GetDelegate<Fn_Int64_Int64Int64>(proc)(a, b);
                }

                case Signature.UIntVoid:
                {
                    uint r = stdcall
                        ? GetDelegate<Fn_UIntVoid_S>(proc)()
                        : GetDelegate<Fn_UIntVoid>(proc)();
                    return r + " (0x" + r.ToString("X") + ")";
                }

                case Signature.DoubleVoid:
                    return stdcall
                        ? GetDelegate<Fn_DoubleVoid_S>(proc)()
                        : GetDelegate<Fn_DoubleVoid>(proc)();

                case Signature.DoubleDoubleDouble:
                {
                    double a = ParseDouble(txtArg1.Text, "Arg 1");
                    double b = ParseDouble(txtArg2.Text, "Arg 2");
                    return stdcall
                        ? GetDelegate<Fn_DoubleDoubleDouble_S>(proc)(a, b)
                        : GetDelegate<Fn_DoubleDoubleDouble>(proc)(a, b);
                }

                case Signature.FloatFloat:
                {
                    float a = ParseFloat(txtArg1.Text, "Arg 1");
                    return stdcall
                        ? GetDelegate<Fn_FloatFloat_S>(proc)(a)
                        : GetDelegate<Fn_FloatFloat>(proc)(a);
                }

                case Signature.StrVoid:
                {
                    IntPtr p = stdcall
                        ? GetDelegate<Fn_StrVoid_S>(proc)()
                        : GetDelegate<Fn_StrVoid>(proc)();
                    string s = p == IntPtr.Zero ? "(null)" : Marshal.PtrToStringAnsi(p);
                    return "\"" + s + "\"  (ptr 0x" + p.ToInt64().ToString("X") + ")";
                }

                case Signature.IntStr:
                {
                    string arg = txtArg1.Text;
                    return stdcall
                        ? GetDelegate<Fn_IntStr_S>(proc)(arg)
                        : GetDelegate<Fn_IntStr>(proc)(arg);
                }

                case Signature.StrStr:
                {
                    string arg = txtArg1.Text;
                    IntPtr p = stdcall
                        ? GetDelegate<Fn_StrStr_S>(proc)(arg)
                        : GetDelegate<Fn_StrStr>(proc)(arg);
                    string s = p == IntPtr.Zero ? "(null)" : Marshal.PtrToStringAnsi(p);
                    return "\"" + s + "\"  (ptr 0x" + p.ToInt64().ToString("X") + ")";
                }

                case Signature.WStrVoid:
                {
                    IntPtr p = stdcall
                        ? GetDelegate<Fn_WStrVoid_S>(proc)()
                        : GetDelegate<Fn_WStrVoid>(proc)();
                    string s = p == IntPtr.Zero ? "(null)" : Marshal.PtrToStringUni(p);
                    return "\"" + s + "\"  (ptr 0x" + p.ToInt64().ToString("X") + ")";
                }

                case Signature.IntWStr:
                {
                    string arg = txtArg1.Text;
                    return stdcall
                        ? GetDelegate<Fn_IntWStr_S>(proc)(arg)
                        : GetDelegate<Fn_IntWStr>(proc)(arg);
                }

                case Signature.WStrWStr:
                {
                    string arg = txtArg1.Text;
                    IntPtr p = stdcall
                        ? GetDelegate<Fn_WStrWStr_S>(proc)(arg)
                        : GetDelegate<Fn_WStrWStr>(proc)(arg);
                    string s = p == IntPtr.Zero ? "(null)" : Marshal.PtrToStringUni(p);
                    return "\"" + s + "\"  (ptr 0x" + p.ToInt64().ToString("X") + ")";
                }

                case Signature.IntWStrInt:
                {
                    string arg = txtArg1.Text;
                    int i = ParseInt(txtArg2.Text, "Arg 2");
                    return stdcall
                        ? GetDelegate<Fn_IntWStrInt_S>(proc)(arg, i)
                        : GetDelegate<Fn_IntWStrInt>(proc)(arg, i);
                }

                case Signature.BStrVoid:
                {
                    string s = stdcall
                        ? GetDelegate<Fn_BStrVoid_S>(proc)()
                        : GetDelegate<Fn_BStrVoid>(proc)();
                    return "\"" + (s ?? "(null)") + "\"";
                }

                case Signature.VoidPtrVoid:
                {
                    IntPtr p = stdcall
                        ? GetDelegate<Fn_VoidPtrVoid_S>(proc)()
                        : GetDelegate<Fn_VoidPtrVoid>(proc)();
                    return "0x" + p.ToInt64().ToString("X");
                }

                case Signature.IntPtrArg:
                {
                    IntPtr arg = (IntPtr)ParseLong(txtArg1.Text, "Arg 1 (pointer)");
                    return stdcall
                        ? GetDelegate<Fn_IntPtrArg_S>(proc)(arg)
                        : GetDelegate<Fn_IntPtrArg>(proc)(arg);
                }

                case Signature.BoolBool:
                {
                    bool a = ParseBool(txtArg1.Text, "Arg 1");
                    return stdcall
                        ? GetDelegate<Fn_BoolBool_S>(proc)(a)
                        : GetDelegate<Fn_BoolBool>(proc)(a);
                }

                default:
                    throw new NotSupportedException("Unknown signature.");
            }
        }

        private static int ParseInt(string text, string field)
        {
            text = (text ?? "").Trim();
            if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                int hv;
                if (int.TryParse(text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out hv))
                    return hv;
            }
            int v;
            if (int.TryParse(text, out v))
                return v;
            throw new FormatException(field + " is not a valid integer: '" + text + "'");
        }

        private static long ParseLong(string text, string field)
        {
            text = (text ?? "").Trim();
            if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                long hv;
                if (long.TryParse(text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out hv))
                    return hv;
            }
            long v;
            if (long.TryParse(text, out v))
                return v;
            throw new FormatException(field + " is not a valid 64-bit integer: '" + text + "'");
        }

        private static double ParseDouble(string text, string field)
        {
            double v;
            if (double.TryParse((text ?? "").Trim(),
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out v))
                return v;
            throw new FormatException(field + " is not a valid number: '" + text + "'");
        }

        private static float ParseFloat(string text, string field)
        {
            float v;
            if (float.TryParse((text ?? "").Trim(),
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out v))
                return v;
            throw new FormatException(field + " is not a valid number: '" + text + "'");
        }

        private static bool ParseBool(string text, string field)
        {
            text = (text ?? "").Trim().ToLowerInvariant();
            if (text == "true" || text == "1" || text == "yes") return true;
            if (text == "false" || text == "0" || text == "no" || text == "") return false;
            throw new FormatException(field + " is not a valid bool (use true/false or 1/0): '" + text + "'");
        }

        // ---- UI helpers ----
        private void cmbSignature_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateArgVisibility();
        }

        private void UpdateArgVisibility()
        {
            var sig = (Signature)cmbSignature.SelectedIndex;
            bool a1 = false, a2 = false;
            string l1 = "Arg 1:", l2 = "Arg 2:";

            switch (sig)
            {
                case Signature.IntInt:             a1 = true; l1 = "int a:"; break;
                case Signature.IntIntInt:          a1 = a2 = true; l1 = "int a:"; l2 = "int b:"; break;
                case Signature.Int64_Int64Int64:   a1 = a2 = true; l1 = "int64 a:"; l2 = "int64 b:"; break;
                case Signature.DoubleDoubleDouble: a1 = a2 = true; l1 = "double a:"; l2 = "double b:"; break;
                case Signature.FloatFloat:         a1 = true; l1 = "float x:"; break;
                case Signature.IntStr:             a1 = true; l1 = "char* s:"; break;
                case Signature.StrStr:             a1 = true; l1 = "char* s:"; break;
                case Signature.IntWStr:            a1 = true; l1 = "wchar* s:"; break;
                case Signature.WStrWStr:           a1 = true; l1 = "wchar* s:"; break;
                case Signature.IntWStrInt:         a1 = a2 = true; l1 = "wchar* s:"; l2 = "int i:"; break;
                case Signature.IntPtrArg:          a1 = true; l1 = "ptr (hex):"; break;
                case Signature.BoolBool:           a1 = true; l1 = "bool b:"; break;
            }

            lblArg1.Visible = txtArg1.Visible = a1;
            lblArg2.Visible = txtArg2.Visible = a2;
            lblArg1.Text = l1;
            lblArg2.Text = l2;
        }

        private void UpdateModuleStatus()
        {
            if (_hModule == IntPtr.Zero)
            {
                lblModuleStatus.Text = "Not loaded";
                lblModuleStatus.ForeColor = Color.DimGray;
            }
            else
            {
                lblModuleStatus.Text = "Loaded  (handle 0x" + _hModule.ToInt64().ToString("X") + ")";
                lblModuleStatus.ForeColor = Color.ForestGreen;
            }
        }

        private void Log(string msg)
        {
            txtOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + msg + Environment.NewLine);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_hModule != IntPtr.Zero)
            {
                FreeLibrary(_hModule);
                _hModule = IntPtr.Zero;
            }
            base.OnFormClosed(e);
        }
    }
}
