using System;
using com.bloomberg.emsx.samples;
using System.Globalization;

namespace EMSXStateTransitionCapture
{
    class EMSXStateTransitionCapture : NotificationHandler
    {

        EasyMSX emsx;

        static void Main(string[] args)
        {
            EMSXStateTransitionCapture Test = new EMSXStateTransitionCapture();
            Test.Run();

            System.Console.WriteLine("Press enter to terminate...");
            System.Console.ReadLine();
            System.Console.WriteLine("Terminating.");
        }

        private void Run()
        {

            this.emsx = new EasyMSX();
            this.emsx.routes.addNotificationHandler(this);
            this.emsx.orders.addNotificationHandler(this);
            this.emsx.start();
        }

        public void processNotification(Notification notification)
        {

            if (notification.category == Notification.NotificationCategory.ORDER)
            {
                System.Console.WriteLine("ORDER: " + notification.getOrder().field("EMSX_SEQUENCE").value() + ","
                    + notification.getOrder().field("EMSX_STATUS").previousValue() + ","
                    + notification.getOrder().field("EMSX_STATUS").value() + ","
                    + notification.getOrder().field("EMSX_WORKING").previousValue() + ","
                    + notification.getOrder().field("EMSX_WORKING").value() + ",");

            }
            else if (notification.category == Notification.NotificationCategory.ROUTE)
            {

                System.Console.WriteLine("ROUTE: " + notification.getRoute().field("EMSX_SEQUENCE").value() + "." + notification.getRoute().field("EMSX_ROUTE_ID").value() + ","
                    + notification.getRoute().field("EMSX_STATUS").previousValue() + ","
                    + notification.getRoute().field("EMSX_STATUS").value() + ","
                    + notification.getRoute().field("EMSX_WORKING").previousValue() + ","
                    + notification.getRoute().field("EMSX_WORKING").value() + ","
                    + notification.getRoute().field("EMSX_BROKER_STATUS").previousValue() + ","
                    + notification.getRoute().field("EMSX_BROKER_STATUS").value() + ",");
            }

        }
    }
}
