using Nancy.Json.Simple;
using NancyMusicStore.Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NancyMusicStore.Messaging
{
    public class BasicPublisher : IBasicPublisher
    {
        private const string exchangename = "musicstore-orders-ex";
        private const string queuename = "musicstore-orders";
        private const string replyqueuename = "musicstore-orders-reply";
        private const string routingkey = "musicstore-order";
        private readonly IModel channel;
        private readonly IDbHelper dbhelper;
        private readonly AppSettings settings;
        public BasicPublisher(IDbHelper dbHelper, AppSettings settings)
        {
            this.settings = settings;
            this.dbhelper = dbHelper;
            ConnectionFactory factory = new ConnectionFactory();
            factory.SetUri(new Uri(settings.RabbitUri));
            IConnection conn = factory.CreateConnection();
            
            channel = conn.CreateModel();
            channel.ExchangeDeclare(exchangename, ExchangeType.Direct);
            channel.QueueDeclare(queuename, true, false, false, null);
            channel.QueueDeclare(replyqueuename, true, false, false, null);
            channel.QueueBind(queuename, exchangename, routingkey);
            HandleOrderUpdates();
        }

        public void SendMessage(object message, string correlation = null)
        {
            IBasicProperties props = channel.CreateBasicProperties();
            props.CorrelationId = correlation;
            Log.Logger.Information("Publishing message {Id}", props.CorrelationId);
            var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            channel.BasicPublish(exchangename, routingkey, props, buffer);

        }
        public void HandleOrderUpdates()
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, message) =>
            {
                var order = JsonConvert.DeserializeObject<dynamic>(Encoding.UTF8.GetString(message.Body));

                dbhelper.Execute(Queries.AddOrderShippingId, new { shipno = (int)order.ID, oid = (int)order.ordernumber });
                Log.Logger.Information("Processing message {messageId} for order #{ordernumber} and shipment tracking #{trackingno}", message.BasicProperties.CorrelationId, (int)order.ordernumber, (int)order.ID);
                channel.BasicAck(message.DeliveryTag, false);
            };
            channel.BasicConsume(replyqueuename, false, consumer);
        }




    }
}
