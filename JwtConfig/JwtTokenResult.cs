public class JwtTokenResult
    {
        public string access_token { get; set; } = null!;
        public string refresh_token { get; set; } = null!;
        /// <summary>
        /// 过期时间(单位秒)
        /// </summary>
        public int expires_in { get; set; }
        public string token_type { get; set; } = null!;
        public UserInfo user { get; set; } = null!;
    }