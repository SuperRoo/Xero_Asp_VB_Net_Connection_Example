I create ASP.NET applications and use VB.NET...since zero couldn't give an example in this blend I have modified the  examples that they have given (XeroApi.VBConsole and XeroApi.MvcWebApp) to achieve a result.

It stores the access and request tokens in a session variable that you can access via the SessionManager.XeroTokenRepository...there are comments in the code to explain how to connect up to a database for permanent storage.

Using the getCurrentSession property looks like it will use the session tokens to recreate the session, but I haven't tested this.

Remember all care but no responsibility.

Created by Roger Burke of Star1 P/L

