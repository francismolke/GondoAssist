using System.Threading.Tasks;
using WordPressPCL.Models;

namespace WordPressPCL.Tests.Selfhosted.Utility
{
    public static class ClientHelper
    {
        private static WordPressClient _client;
        private static WordPressClient _clientAuth;

        public static async Task<WordPressClient> GetAuthenticatedWordPressClient(AuthMethod method = AuthMethod.JWT)
        {
            if (_clientAuth == null)
            {
                _clientAuth = new WordPressClient("http://www.slamdank31.com/wp-json/")
                {
                    AuthMethod = AuthMethod.JWT
                };
                await _clientAuth.RequestJWToken("Arschloch1", "Slimtwix13");
            }

            return _clientAuth;
        }

        public static WordPressClient GetWordPressClient()
        {
            if (_client == null)
                _client = new WordPressClient("http://www.slamdank31.com/wp-json/");
            return _client;
        }
    }
}












//using System.Threading.Tasks;
//using WordPressPCL.Models;

//namespace WordPressPCL.Tests.Selfhosted.Utility
//{
//    public static class ClientHelper
//    {
//        private static WordPressClient _client;
//        private static WordPressClient _clientAuth;

//        public static async Task<WordPressClient> GetAuthenticatedWordPressClient(AuthMethod method = AuthMethod.JWT)
//        {
//            if (_clientAuth == null)
//            {
//                _clientAuth = new WordPressClient(ApiCredentials.WordPressUri)
//                {
//                    AuthMethod = AuthMethod.JWT
//                };
//                await _clientAuth.RequestJWToken(ApiCredentials.Username, ApiCredentials.Password);
//            }

//            return _clientAuth;
//        }

//        public static WordPressClient GetWordPressClient()
//        {
//            if (_client == null)
//                _client = new WordPressClient(ApiCredentials.WordPressUri);
//            return _client;
//        }
//    }
//}
