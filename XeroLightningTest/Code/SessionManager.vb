Imports XeroApi
Imports DevDefined.OAuth.Consumer
Public Class SessionManager
    Private Const XeroSessionKey As String = "XeroSessionIDKey"
    Private Const XeroRepositoryKey As String = "XeroRepositoryIDKey"
    Private Const XeroTokenRepositoryKey As String = "XeroTokenRepositoryIDKey"
    Private Shared ReadOnly Property Session As HttpSessionState
        Get
            Return HttpContext.Current.Session
        End Get

    End Property

    Public Shared Property XeroRepository As Repository
        Get
            Return CType(Session(XeroRepositoryKey), Repository)
        End Get
        Set(value As Repository)
            Session(XeroRepositoryKey) = value
        End Set
    End Property
    Public Shared Property XeroSession As IOAuthSession
        Get
            Return If(CType(Session(XeroSessionKey), IOAuthSession), (InlineAssignHelper(CType(Session(XeroSessionKey), IOAuthSession), XeroManager.CreateOAuthSession())))
        End Get
        Set(value As IOAuthSession)
            Session(XeroSessionKey) = value
        End Set
    End Property
    Public Shared Property XeroTokenRepository As TokenRepository
        Get
            If Session(XeroTokenRepositoryKey) Is Nothing Then
                Return New TokenRepository
            Else
                Return CType(Session(XeroTokenRepositoryKey), TokenRepository)
            End If
        End Get
        Set(value As TokenRepository)
            Session(XeroSessionKey) = value
        End Set
    End Property
   
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function
End Class
