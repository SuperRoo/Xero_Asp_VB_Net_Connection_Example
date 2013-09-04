Imports System.Configuration.ConfigurationManager
Public Class ConfigManager
    Public Shared ReadOnly Property ConsumerKey As String
        Get
            Return AppSettings("XeroApiConsumerKey")
        End Get
    End Property
    Public Shared ReadOnly Property ConsumerSecret As String
        Get
            Return AppSettings("XeroApiConsumerSecret")
        End Get
    End Property
    Public Shared ReadOnly Property UserAgent As String
        Get
            Return AppSettings("XeroUserAgent")
        End Get
    End Property
End Class
