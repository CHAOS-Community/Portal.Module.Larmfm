namespace Chaos.Portal.Module.Larmfm.Domain.Aws
{
    using Amazon;
    using Amazon.ElasticTranscoder.Model;

    public class ElasticTranscoder : ITranscoder
    {
        public LarmSettings.AwsSettings Settings { get; set; }

        public ElasticTranscoder(LarmSettings.AwsSettings settings)
        {
            Settings = settings;
        }

        public void Transcode(string inputKey, string outputKey)
        {
            using (var et = AWSClientFactory.CreateAmazonElasticTranscoderClient(Settings.AccessKey, Settings.SecretAccessKey, RegionEndpoint.EUWest1))
            {
                var request = new CreateJobRequest
                {
                    PipelineId = Settings.PipelineId,
                    Input = new JobInput
                    {
                        Key = inputKey
                    },
                    Output = new CreateJobOutput
                    {
                        PresetId = Settings.PresetId,
                        Key = outputKey
                    }
                };
                et.CreateJob(request);
            }
        }
    }
}
