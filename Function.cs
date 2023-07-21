using Amazon;
using Amazon.Lambda.Core;
using Amazon.S3.Transfer;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CVBuilderMaintenanceLambda;

/// Lambda name -> 'cv-builder-maintenance'
public class Function
{
    #region Helper Classes

    private class MaintenanceInfo
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string? Description { get; set; }
        public int Severity { get; set; }
    }

    #endregion

    private const string BucketName = "cv-builder-storage";
    private const string Region = "eu-west-1";
    private const string MaintenanceFileName = "MaintenanceSchedule.json";

    private ITransferUtility _transfer;

    public Function()
    {
        _transfer = new TransferUtility(RegionEndpoint.GetBySystemName(Region));
    }

    public string FunctionHandler(ILambdaContext context)
    {
        try
        {
            Stream fileStream = _transfer.OpenStream(BucketName, MaintenanceFileName);
            StreamReader reader = new(fileStream);
            string response = reader.ReadToEnd();
            LambdaLogger.Log(response);
            return response;
        }
        catch(Exception ex)
        {
            LambdaLogger.Log($"Failed to execute function: {ex.Message}");
            return "[]";
        }
    }
}
