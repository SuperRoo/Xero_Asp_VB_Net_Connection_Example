Imports XeroApi
Imports DevDefined.OAuth.Consumer
Imports XeroApi.OAuth
Imports DevDefined.OAuth.Storage.Basic
Imports XeroApi.Model

Public Class _Default
    Inherits System.Web.UI.Page
    Private oRepository As Repository
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.hidFlag.Value = "" Then
            Me.hidFlag.Value = "set"
            If Request.QueryString("oauth_verifier") <> "" Then
                CallBack(Request.QueryString("oauth_verifier"))
            End If
        End If

    End Sub

    Private Sub cmdLogin_Command(sender As Object, e As System.Web.UI.WebControls.CommandEventArgs) Handles cmdLogin.Command
        Login()
    End Sub
    Private Sub Login()
        'if storing access and request tokens in database then load the session here
        'Dim td As New TokenRepository
        'td.SaveAccessToken = accesstoken
        'td.SaveRequestToken = request
        'SessionManager.XeroTokenRepository = td
        ' save access & request in callback after getting access token in callback
        SessionManager.XeroSession = XeroManager.GetCurrentSession
        ' Determine the callback uri to use - this must match the domain used when the application was registerd on http://api.xero.com
        'this calls back the Default.aspx page...check for the querystring oauth_verifier
        Dim callbackUri = New UriBuilder(Request.Url.Scheme, Request.Url.Host, Request.Url.Port, "Default.aspx")

        ' Call: GET /oauth/RequestToken
        Dim token As RequestToken = SessionManager.XeroSession.GetRequestToken(callbackUri.Uri)

        Dim authorisationUrl As String = SessionManager.XeroSession.GetUserAuthorizationUrl()
        Response.Redirect(authorisationUrl)
    End Sub
    Private Sub CallBack(verificationCode As String)
        Dim sb As New StringBuilder
        Dim oauthSession As IOAuthSession = SessionManager.XeroSession
        Dim accesstkn As AccessToken = oauthSession.ExchangeRequestTokenForAccessToken(verificationCode)
        'save SessionManager.XeroTokenRepository here for database
        SessionManager.XeroRepository = New Repository(oauthSession)
        Me.lblCompanyName.Text = "Comnapny Name: " & SessionManager.XeroRepository.Organisation.Name
        Me.cmdLogin.Visible = False
        Dim rep As Repository = SessionManager.XeroRepository
        For Each con As Contact In rep.Contacts
            If con.IsCustomer Then
                sb.Append(con.Name & vbCrLf)
            End If
        Next
        Me.txtCustomers.Text = sb.ToString
    End Sub
End Class