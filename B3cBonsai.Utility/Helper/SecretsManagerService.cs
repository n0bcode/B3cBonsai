using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;

namespace B3cBonsai.Utility.Helper
{
    public class SecretsManagerService
    {
        private readonly IAmazonSecretsManager _client;
        private readonly string _secretArn;

        public SecretsManagerService(IConfiguration configuration)
        {
            // Lấy các tùy chọn AWS từ cấu hình
            var awsOptions = configuration.GetSection("AWS").Get<AwsOptions>();
            _secretArn = awsOptions.SecretArn;

            // Tạo thông tin xác thực AWS
            var credentials = new BasicAWSCredentials(awsOptions.AccessKeyId, awsOptions.SecretAccessKey);
            _client = new AmazonSecretsManagerClient(credentials, RegionEndpoint.GetBySystemName(awsOptions.Region));
        }

        public async Task<Dictionary<string, string>> GetSecretAsync()
        {
            var request = new GetSecretValueRequest
            {
                SecretId = _secretArn,
                VersionStage = "AWSCURRENT"
            };

            GetSecretValueResponse response;

            try
            {
                response = await _client.GetSecretValueAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving secret: {ex.Message}");
                throw;
            }

            if (string.IsNullOrEmpty(response.SecretString))
            {
                throw new Exception("SecretString is empty or null.");
            }

            var secretDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.SecretString);
            return secretDict;
        }
    }

    public class AwsOptions
    {
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string Region { get; set; }
        public string SecretArn { get; set; }
    }
}
