using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

[assembly: CLSCompliant(true)]
#if !NO_COM_ATTRIBUTES
[assembly: ComVisible(false)]
#endif
//[assembly: AllowPartiallyTrustedCallers] https://stackoverflow.com/questions/11885764/attempt-by-security-transparent-method-x-to-access-security-critical-method-y-fa
[assembly: AssemblyDelaySign(false)]
