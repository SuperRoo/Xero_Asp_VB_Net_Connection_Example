Imports DevDefined.OAuth.Storage.Basic
Public Class TokenRepository
    Implements ITokenRepository
    Private _session As HttpSessionState = HttpContext.Current.Session


    Public Function GetAccessToken() As Global.DevDefined.OAuth.Storage.Basic.AccessToken Implements Global.DevDefined.OAuth.Storage.Basic.ITokenRepository.GetAccessToken
        Try
            Return _session("xeroAccessTokenID")
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function GetRequestToken() As Global.DevDefined.OAuth.Storage.Basic.RequestToken Implements Global.DevDefined.OAuth.Storage.Basic.ITokenRepository.GetRequestToken
        Try
            Return CType(_session("xeroRequestTokenID"), RequestToken)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Sub SaveAccessToken(accessToken As Global.DevDefined.OAuth.Storage.Basic.AccessToken) Implements Global.DevDefined.OAuth.Storage.Basic.ITokenRepository.SaveAccessToken
        _session("xeroAccessTokenID") = accessToken
    End Sub

    Public Sub SaveRequestToken(requestToken As Global.DevDefined.OAuth.Storage.Basic.RequestToken) Implements Global.DevDefined.OAuth.Storage.Basic.ITokenRepository.SaveRequestToken
        _session("xeroRequestTokenID") = requestToken
    End Sub
End Class
