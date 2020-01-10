using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Xml_DB_SQS
{
    class Program
    {


        static void Main(string[] args)
        {

            StoredProfileAWSCredentials credentials = new StoredProfileAWSCredentials("development", @"C:\Users\YaligarN\.aws\credentials");

            var dbclient = new AmazonDynamoDBClient(credentials,RegionEndpoint.APSouth1);
            var access = new AmazonSQSClient(credentials,RegionEndpoint.APSouth1);

            //var Credentials = new Amazon.Runtime.BasicAWSCredentials("AKIAJ3ZHDUXWC4HLNURA", "vVxclYGcoUFSG0Mx0eBYcE7D839Podwboj/BJS4v");
            //var dbclient = new Amazon.DynamoDBv2.AmazonDynamoDBClient(RegionEndpoint.APSouth1);
            //var access = new Amazon.SQS.AmazonSQSClient(Amazon.RegionEndpoint.APSouth1);

            string queueUrl = "https://sqs.ap-south-1.amazonaws.com/068090245287/Employee";
            Console.WriteLine("Its connecting to SQS");
            ReceiveMessageRequest request = new ReceiveMessageRequest(queueUrl);
            request.MaxNumberOfMessages = 3;
            ReceiveMessageResponse response = access.ReceiveMessageAsync(request).Result;
            Console.WriteLine("Message is received");
            foreach(var message in response.Messages)
            {
                string xml = message.Body;
            Table table = Table.LoadTable(dbclient, "Employee");
            XmlSerializer ser = new XmlSerializer(typeof(Demo));
            Demo Employee = (Demo)ser.Deserialize(new StringReader(xml));
            string json = JsonConvert.SerializeObject(Employee, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            var item = Document.FromJson(json);
            table.PutItemAsync(item);
            }
            Console.ReadLine();
        }
    }

    [XmlType("Employee")]
    public class Demo
    {
        public int Emp_ID { get; set; }
        public string Emp_Name { get; set; }
        public int Emp_Salary { get; set; }

    }
}
