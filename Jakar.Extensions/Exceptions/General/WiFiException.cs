﻿// unset


namespace Jakar.Extensions;


public class WiFiException : Exception
{
    public WiFiException() { }
    public WiFiException( string message ) : base( message ) { }
    public WiFiException( string message, Exception inner ) : base( message, inner ) { }
}
