using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Harden")]
[assembly: AssemblyDescription("Wraps your classes with a proxy which automatically carries out permission checks when you call methods.\r\ne.g. If you have a method \r\n'public virtual void SendEmail()'\r\n, you can create a method \r\n'public virtual bool' AllowSendEmail() { ... }\r\nWhen SendEmail is called, AllowSendEmail is called first. If this returns false, a HardenException is thrown. To harden an object, call Hardener.Harden(someObject). ")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Harry McIntyre")]
[assembly: AssemblyProduct("Harden")]
[assembly: AssemblyCopyright("MIT Licence")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("65b85492-11c4-479a-85a6-69b7a8aa3374")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyFileVersion("1.0.0.0")]
