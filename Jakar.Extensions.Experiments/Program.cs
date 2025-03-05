using System.Text;


Console.WriteLine( DateTimeOffset.UtcNow.ToString() );


UInt128 number = UInt128.Parse( "7039212526715952524126581231157018804" );
string  str    = number.ToString();




str.Hash().WriteToDebug();
str.Hash( Encoding.Default ).WriteToDebug();
str.Hash128().WriteToDebug();
str.Hash_MD5().WriteToDebug();
str.Hash_SHA1().WriteToDebug();
str.Hash_SHA256().WriteToDebug();
str.Hash_SHA384().WriteToDebug();
str.Hash_SHA512().WriteToDebug(); 
