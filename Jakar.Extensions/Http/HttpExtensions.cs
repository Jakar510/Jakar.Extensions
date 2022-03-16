﻿namespace Jakar.Extensions.Http;


public static class HttpExtensions
{
    public static HttpRequestHeader? ToHttpRequestHeader( this string header ) => header.Trim() switch
                                                                                  {
                                                                                      nameof(HttpRequestHeader.Accept)             => HttpRequestHeader.Accept,
                                                                                      nameof(HttpRequestHeader.AcceptCharset)      => HttpRequestHeader.AcceptCharset,
                                                                                      nameof(HttpRequestHeader.AcceptEncoding)     => HttpRequestHeader.AcceptLanguage,
                                                                                      nameof(HttpRequestHeader.AcceptLanguage)     => HttpRequestHeader.AcceptLanguage,
                                                                                      nameof(HttpRequestHeader.Allow)              => HttpRequestHeader.Allow,
                                                                                      nameof(HttpRequestHeader.Authorization)      => HttpRequestHeader.Authorization,
                                                                                      nameof(HttpRequestHeader.CacheControl)       => HttpRequestHeader.CacheControl,
                                                                                      nameof(HttpRequestHeader.Connection)         => HttpRequestHeader.Connection,
                                                                                      nameof(HttpRequestHeader.ContentEncoding)    => HttpRequestHeader.ContentEncoding,
                                                                                      nameof(HttpRequestHeader.ContentLanguage)    => HttpRequestHeader.ContentLanguage,
                                                                                      nameof(HttpRequestHeader.ContentLength)      => HttpRequestHeader.ContentLength,
                                                                                      nameof(HttpRequestHeader.ContentLocation)    => HttpRequestHeader.ContentLanguage,
                                                                                      nameof(HttpRequestHeader.ContentMd5)         => HttpRequestHeader.ContentMd5,
                                                                                      nameof(HttpRequestHeader.ContentRange)       => HttpRequestHeader.ContentRange,
                                                                                      nameof(HttpRequestHeader.ContentType)        => HttpRequestHeader.ContentType,
                                                                                      nameof(HttpRequestHeader.Cookie)             => HttpRequestHeader.Cookie,
                                                                                      nameof(HttpRequestHeader.Date)               => HttpRequestHeader.Date,
                                                                                      nameof(HttpRequestHeader.Expect)             => HttpRequestHeader.Expect,
                                                                                      nameof(HttpRequestHeader.Expires)            => HttpRequestHeader.Expires,
                                                                                      nameof(HttpRequestHeader.From)               => HttpRequestHeader.From,
                                                                                      nameof(HttpRequestHeader.Host)               => HttpRequestHeader.Host,
                                                                                      nameof(HttpRequestHeader.IfMatch)            => HttpRequestHeader.IfMatch,
                                                                                      nameof(HttpRequestHeader.IfModifiedSince)    => HttpRequestHeader.IfModifiedSince,
                                                                                      nameof(HttpRequestHeader.IfNoneMatch)        => HttpRequestHeader.IfNoneMatch,
                                                                                      nameof(HttpRequestHeader.IfRange)            => HttpRequestHeader.IfRange,
                                                                                      nameof(HttpRequestHeader.IfUnmodifiedSince)  => HttpRequestHeader.IfModifiedSince,
                                                                                      nameof(HttpRequestHeader.KeepAlive)          => HttpRequestHeader.KeepAlive,
                                                                                      nameof(HttpRequestHeader.LastModified)       => HttpRequestHeader.LastModified,
                                                                                      nameof(HttpRequestHeader.MaxForwards)        => HttpRequestHeader.MaxForwards,
                                                                                      nameof(HttpRequestHeader.Pragma)             => HttpRequestHeader.Pragma,
                                                                                      nameof(HttpRequestHeader.ProxyAuthorization) => HttpRequestHeader.ProxyAuthorization,
                                                                                      nameof(HttpRequestHeader.Range)              => HttpRequestHeader.Range,
                                                                                      nameof(HttpRequestHeader.Referer)            => HttpRequestHeader.Referer,
                                                                                      nameof(HttpRequestHeader.Te)                 => HttpRequestHeader.Te,
                                                                                      nameof(HttpRequestHeader.Trailer)            => HttpRequestHeader.Trailer,
                                                                                      nameof(HttpRequestHeader.TransferEncoding)   => HttpRequestHeader.TransferEncoding,
                                                                                      nameof(HttpRequestHeader.Translate)          => HttpRequestHeader.Translate,
                                                                                      nameof(HttpRequestHeader.Upgrade)            => HttpRequestHeader.Upgrade,
                                                                                      nameof(HttpRequestHeader.UserAgent)          => HttpRequestHeader.UserAgent,
                                                                                      nameof(HttpRequestHeader.Via)                => HttpRequestHeader.Via,
                                                                                      nameof(HttpRequestHeader.Warning)            => HttpRequestHeader.Warning,
                                                                                      _                                            => null
                                                                                  };

    public static string ToFastString( this HttpRequestHeader header ) => header switch
                                                                          {
                                                                              HttpRequestHeader.Accept             => nameof(HttpRequestHeader.Accept),
                                                                              HttpRequestHeader.AcceptCharset      => nameof(HttpRequestHeader.AcceptCharset),
                                                                              HttpRequestHeader.AcceptEncoding     => nameof(HttpRequestHeader.AcceptEncoding),
                                                                              HttpRequestHeader.AcceptLanguage     => nameof(HttpRequestHeader.AcceptLanguage),
                                                                              HttpRequestHeader.Allow              => nameof(HttpRequestHeader.Allow),
                                                                              HttpRequestHeader.Authorization      => nameof(HttpRequestHeader.Authorization),
                                                                              HttpRequestHeader.CacheControl       => nameof(HttpRequestHeader.CacheControl),
                                                                              HttpRequestHeader.Connection         => nameof(HttpRequestHeader.Connection),
                                                                              HttpRequestHeader.ContentEncoding    => nameof(HttpRequestHeader.ContentEncoding),
                                                                              HttpRequestHeader.ContentLanguage    => nameof(HttpRequestHeader.ContentLanguage),
                                                                              HttpRequestHeader.ContentLength      => nameof(HttpRequestHeader.ContentLength),
                                                                              HttpRequestHeader.ContentLocation    => nameof(HttpRequestHeader.ContentLocation),
                                                                              HttpRequestHeader.ContentMd5         => nameof(HttpRequestHeader.ContentMd5),
                                                                              HttpRequestHeader.ContentRange       => nameof(HttpRequestHeader.ContentRange),
                                                                              HttpRequestHeader.ContentType        => nameof(HttpRequestHeader.ContentType),
                                                                              HttpRequestHeader.Cookie             => nameof(HttpRequestHeader.Cookie),
                                                                              HttpRequestHeader.Date               => nameof(HttpRequestHeader.Date),
                                                                              HttpRequestHeader.Expect             => nameof(HttpRequestHeader.Expect),
                                                                              HttpRequestHeader.Expires            => nameof(HttpRequestHeader.Expires),
                                                                              HttpRequestHeader.From               => nameof(HttpRequestHeader.From),
                                                                              HttpRequestHeader.Host               => nameof(HttpRequestHeader.Host),
                                                                              HttpRequestHeader.IfMatch            => nameof(HttpRequestHeader.IfMatch),
                                                                              HttpRequestHeader.IfModifiedSince    => nameof(HttpRequestHeader.IfModifiedSince),
                                                                              HttpRequestHeader.IfNoneMatch        => nameof(HttpRequestHeader.IfNoneMatch),
                                                                              HttpRequestHeader.IfRange            => nameof(HttpRequestHeader.IfRange),
                                                                              HttpRequestHeader.IfUnmodifiedSince  => nameof(HttpRequestHeader.IfUnmodifiedSince),
                                                                              HttpRequestHeader.KeepAlive          => nameof(HttpRequestHeader.KeepAlive),
                                                                              HttpRequestHeader.LastModified       => nameof(HttpRequestHeader.LastModified),
                                                                              HttpRequestHeader.MaxForwards        => nameof(HttpRequestHeader.MaxForwards),
                                                                              HttpRequestHeader.Pragma             => nameof(HttpRequestHeader.Pragma),
                                                                              HttpRequestHeader.ProxyAuthorization => nameof(HttpRequestHeader.ProxyAuthorization),
                                                                              HttpRequestHeader.Range              => nameof(HttpRequestHeader.Range),
                                                                              HttpRequestHeader.Referer            => nameof(HttpRequestHeader.Referer),
                                                                              HttpRequestHeader.Te                 => nameof(HttpRequestHeader.Te),
                                                                              HttpRequestHeader.Trailer            => nameof(HttpRequestHeader.Trailer),
                                                                              HttpRequestHeader.TransferEncoding   => nameof(HttpRequestHeader.TransferEncoding),
                                                                              HttpRequestHeader.Translate          => nameof(HttpRequestHeader.Translate),
                                                                              HttpRequestHeader.Upgrade            => nameof(HttpRequestHeader.Upgrade),
                                                                              HttpRequestHeader.UserAgent          => nameof(HttpRequestHeader.UserAgent),
                                                                              HttpRequestHeader.Via                => nameof(HttpRequestHeader.Via),
                                                                              HttpRequestHeader.Warning            => nameof(HttpRequestHeader.Warning),
                                                                              _                                    => throw new ArgumentOutOfRangeException(nameof(header), header, null)
                                                                          };
}
