package com.elringus.unitygoogledriveandroid;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;

import net.openid.appauth.AuthorizationException;
import net.openid.appauth.AuthorizationRequest;
import net.openid.appauth.AuthorizationResponse;
import net.openid.appauth.AuthorizationService;
import net.openid.appauth.AuthorizationServiceConfiguration;
import net.openid.appauth.ResponseTypeValues;

public class AuthorizationActivity extends Activity {
    public interface OnAuthorizationResponseListener {
        void onAuthorizationResponse(Boolean isError, String error, String codeVerifier, String redirectUri, String authorizationCode);
    }

    private static final int RC_AUTH = 100;
    private static OnAuthorizationResponseListener responseListener;
    private AuthorizationService authorizationService;

    public static void SetResponseListener (OnAuthorizationResponseListener responseListener)
    {
        AuthorizationActivity.responseListener = responseListener;
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        Intent intent = getIntent();
        String authorizationEndpoint = intent.getStringExtra("authorizationEndpoint");
        String tokenEndpoint = intent.getStringExtra("tokenEndpoint");
        String clientId = intent.getStringExtra("clientId");
        String redirectEndpoint = intent.getStringExtra("redirectEndpoint");
        String scope = intent.getStringExtra("scope");

        Uri authorizationUri = Uri.parse(authorizationEndpoint);
        Uri tokenUri = Uri.parse(tokenEndpoint);
        Uri redirectUri = Uri.parse(redirectEndpoint);

        AuthorizationServiceConfiguration configuration = new AuthorizationServiceConfiguration(authorizationUri, tokenUri);
        AuthorizationRequest.Builder requestBuilder = new AuthorizationRequest.Builder(configuration, clientId, ResponseTypeValues.CODE, redirectUri);
        requestBuilder.setScope(scope);
        AuthorizationRequest request = requestBuilder.build();

        authorizationService = new AuthorizationService(this);
        Intent authIntent = authorizationService.getAuthorizationRequestIntent(request);
        startActivityForResult(authIntent, RC_AUTH);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if (requestCode != RC_AUTH) return;

        AuthorizationResponse response = AuthorizationResponse.fromIntent(data);
        AuthorizationException exception = AuthorizationException.fromIntent(data);

        Boolean isError = exception != null;
        String error = isError ? exception.error : "";
        String authorizationCode = response != null ? response.authorizationCode : "";
        String codeVerifier = response != null ? response.request.codeVerifier : "";
        String redirectUri = response != null ? response.request.redirectUri.toString() : "";

        if (responseListener != null) responseListener.onAuthorizationResponse(isError, error, codeVerifier, redirectUri, authorizationCode);

        finish();
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();

        responseListener = null;
        if (authorizationService != null) authorizationService.dispose();
    }
}
