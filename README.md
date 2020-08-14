# Decisions.Dropbox integration
## Preparation
You need to create your Dropbox application in the developer console: https://www.dropbox.com/developers/apps

For this application you should enable scopes:<br />
&nbsp;&nbsp; ***files.content.read    <br />
&nbsp;&nbsp; files.content.write   <br />
&nbsp;&nbsp; files.metadata.read   <br />
&nbsp;&nbsp; sharing.read    <br />
&nbsp;&nbsp; sharing.write***   <br />

Add Redirect URI, for testing purpose you may use http://localhost/decisions/HandleTokenResponse.aspx 

More information you may find  https://www.dropbox.com/developers/documentation


## Creating a provider for Dropbox in Decisions
1. Go to ***System > Integration > OAuth > Providers***   ,  click ***ADD OAUTH PROVIDER***
  2. Fill the form<br />
&nbsp;&nbsp;   ***OAuth Version***: OAuth2 <br />
&nbsp;&nbsp;   ***Token Request URL***: https://www.dropbox.com/oauth2/token   <br />
&nbsp;&nbsp;   ***Authorize URL***: https://www.dropbox.com/oauth2/authorize <br />
&nbsp;&nbsp;   ***Callback URL***: http://localhost/decisions/HandleTokenResponse.aspxe <br />
&nbsp;&nbsp;   Set ***Default Consumer Key*** and ***Default Consumer Secret Key***<br />
 ![screenshot of sample](https://github.com/Decisions-Modules/Decisions.Dropbox/blob/master/provider.png)


## Getting token
 1. Go to ***System > Integration > OAuth > Tokens*** and click ***CREATE TOKEN***.
  2. Set ***Token Name*** value.
  3. Choose the provider you have created.
  4. Click Request Token. A browser window will be open. Just follow the instructions in it.
![screenshot of sample](https://github.com/Decisions-Modules/Decisions.Dropbox/blob/master/token.png)
