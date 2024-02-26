namespace viki_01.Entities
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public AuthResponse()
        {
        }

        public AuthResponse(TokenPair tokens)
        {
            AccessToken = tokens.AccessToken;
            RefreshToken = tokens.RefreshToken;
        }
        public AuthResponse(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
