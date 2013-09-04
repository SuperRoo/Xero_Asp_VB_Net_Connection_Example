Imports DevDefined.OAuth.Storage.Basic
Imports DevDefined.OAuth.Logging
Imports DevDefined.OAuth.Consumer
Imports XeroApi.OAuth
Imports DevDefined.OAuth.Framework
Imports XeroApi
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates

Public Class XeroManager

    'Const UserAgent As String = "Lightning Service Desk V1.0 (Development)"
    'Const ConsumerKey As String = "M8O5DM3RMQVOEFS3DGXMOZEFR8L83A"
    'Const ConsumerSecret As String = "O0YIYGLF0MVM3BRTYGHNFOONN23CKB"

    ''' <summary>
    ''' Gets the current instance of Repository (used for getting data from the Xero API)
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetCurrentRepository() As Repository
        Dim session = GetCurrentSession()

        Return If((session IsNot Nothing), New Repository(session), Nothing)
    End Function

    ''' <summary>
    ''' Gets the current OAuthSession (used for getting request tokens, access tokens, etc.)
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetCurrentSession() As IOAuthSession
        Return If(SessionManager.XeroSession, (InlineAssignHelper(SessionManager.XeroSession, CreateOAuthSession())))
    End Function


    ''' <summary>
    ''' Creates the OAuth session.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function CreateOAuthSession() As IOAuthSession
        Dim userAgent As String = ConfigManager.UserAgent

        If ConfigurationManager.AppSettings("XeroApiSignatureMethod") = "HMAC-SHA1" Then
            ' Public App
            ' Consumer Key
            ' Consumer Secret
            ' Token Repository
            Return New XeroApiPublicSession(userAgent, ConfigurationManager.AppSettings("XeroApiConsumerKey"), ConfigurationManager.AppSettings("XeroApiConsumerSecret"), SessionManager.XeroTokenRepository)
        End If

        If ConfigurationManager.AppSettings("XeroApiSignatureMethod") = "RSA-SHA1" Then
            If ConfigurationManager.AppSettings("XeroApiBaseUrl").ToLower().IndexOf("partner") > 0 Then
                ' Partner App
                ' Consumer Key
                ' OAuth Signing Certificate
                ' Client SSL Certificate
                ' Token Repository
                Return New XeroApiPartnerSession(userAgent, ConfigurationManager.AppSettings("XeroApiConsumerKey"), GetOAuthSigningCertificate(), GetClientSslCertificate(), SessionManager.XeroTokenRepository)
            Else
                ' Private App
                ' Consumer Key
                ' OAuth Signing Certificate
                Return New XeroApiPrivateSession(userAgent, ConfigurationManager.AppSettings("XeroApiConsumerKey"), GetOAuthSigningCertificate())
            End If
        End If

        Throw New ConfigurationErrorsException("The configuration for a Public/Private/Partner app cannot be determined.")
    End Function
    ''' <summary>
    ''' Gets the OAuth signing certificate from the local certificate store, if specfified in app.config.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetOAuthSigningCertificatePrivateKey() As AsymmetricAlgorithm
        Dim oauthSigningCertificate = GetOAuthSigningCertificate()

        If oauthSigningCertificate Is Nothing Then
            Return Nothing
        End If

        If Not oauthSigningCertificate.HasPrivateKey Then
            Throw New ApplicationException("The specified OAuth Certificate find details matched a certificate, but there is not private key stored in the certificate")
        End If

        Return oauthSigningCertificate.PrivateKey
    End Function
    Public Shared Function GetOAuthSigningCertificate() As X509Certificate2
        Dim oauthCertificateFindType As String = ConfigurationManager.AppSettings("XeroApiOAuthCertificateFindType")
        Dim oauthCertificateFindValue As String = ConfigurationManager.AppSettings("XeroApiOAuthCertificateFindValue")

        If String.IsNullOrEmpty(oauthCertificateFindType) OrElse String.IsNullOrEmpty(oauthCertificateFindValue) Then
            Return Nothing
        End If

        Dim x509FindType As X509FindType = DirectCast([Enum].Parse(GetType(X509FindType), oauthCertificateFindType), X509FindType)

        ' Search the LocalMachine certificate store for matching X509 certificates.
        Dim certStore As New X509Store("My", StoreLocation.LocalMachine)
        certStore.Open(OpenFlags.[ReadOnly] Or OpenFlags.OpenExistingOnly)
        Dim certificateCollection As X509Certificate2Collection = certStore.Certificates.Find(x509FindType, oauthCertificateFindValue, False)
        certStore.Close()

        If certificateCollection.Count = 0 Then
            Throw New ApplicationException(String.Format("An OAuth certificate matching the X509FindType '{0}' and Value '{1}' cannot be found in the local certificate store.", oauthCertificateFindType, oauthCertificateFindValue))
        End If

        Return certificateCollection(0)
    End Function

    Public Shared Function GetClientSslCertificate() As X509Certificate2
        Dim clientSslCertificateFactory As ICertificateFactory = GetClientSslCertificateFactory()
        Return clientSslCertificateFactory.CreateCertificate()
    End Function
    ''' <summary>
    ''' Return a CertificateFactory that can read the Client SSL certificate from the local machine certificate store
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetClientSslCertificateFactory() As ICertificateFactory
        Dim oauthCertificateFindType As String = ConfigurationManager.AppSettings("XeroApiSslCertificateFindType")
        Dim oauthCertificateFindValue As String = ConfigurationManager.AppSettings("XeroApiSslCertificateFindValue")

        If String.IsNullOrEmpty(oauthCertificateFindType) OrElse String.IsNullOrEmpty(oauthCertificateFindValue) Then
            Return New NullCertificateFactory()
        End If

        Dim x509FindType As X509FindType = DirectCast([Enum].Parse(GetType(X509FindType), oauthCertificateFindType), X509FindType)
        Dim certificateFactory As ICertificateFactory = New LocalMachineCertificateFactory(oauthCertificateFindValue, x509FindType)

        If certificateFactory.CreateCertificate() Is Nothing Then
            Throw New ApplicationException(String.Format("A client SSL certificate matching the X509FindType '{0}' and value '{1}' cannot be found in the local certificate store.", oauthCertificateFindType, oauthCertificateFindValue))
        End If

        Return certificateFactory
    End Function

    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function
    Private Sub New()
    End Sub

End Class
