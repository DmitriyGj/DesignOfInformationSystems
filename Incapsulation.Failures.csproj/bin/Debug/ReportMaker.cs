using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.Failures
{

    public class Common
    {
        public static int Earlier(object[] v, int day, int month, int year)
        {
            int vYear = (int)v[2];
            int vMonth = (int)v[1];
            int vDay = (int)v[0];
            if (vYear < year) return 1;
            if (vYear > year) return 0;
            if (vMonth < month) return 1;
            if (vMonth > month) return 0;
            if (vDay < day) return 1;
            return 0;
        }
    }
    public class Device
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int FaliureType { get; set; }

        public Device(int id, string name, DateTime date, int failureType)
        {
            this.ID = id;
            this.Name = name;
            this.Date = date;
            this.FaliureType = failureType;
        }
    }

    public class Failure
    {
        public static bool IsFailureSerious(int failureType) 
            => failureType == (int)Failure.FailureTypes.US || failureType == (int)Failure.FailureTypes.HF;

        public enum FailureTypes 
        { 
            US,NR,HF,CP
        }

    }

    public class ReportMaker
    {
        /// <summary>
        /// </summary>
        /// <param name="day"></param>
        /// <param name="failureTypes">
        /// 0 for unexpected shutdown, 
        /// 1 for short non-responding, 
        /// 2 for hardware failures, 
        /// 3 for connection problems
        /// </param>
        /// <param name="deviceId"></param>
        /// <param name="times"></param>
        /// <param name="devices"></param>
        /// <returns></returns>
        public static List<string> FindDevicesFailedBeforeDateObsolete(
            int day,
            int month,
            int year,
            int[] failureTypes, 
            int[] deviceId, 
            object[][] times,
            List<Dictionary<string, object>> devices)
        {
            var currentDate = new DateTime(year,month,day);
            List<Device> parsedDevices = new List<Device>();
            for(int i = 0; i != devices.Count;i++)
            {
                parsedDevices.Add(new Device(
                    (int)devices[i]["DeviceId"],
                    devices[i]["Name"].ToString(),
                    new DateTime((int)times[i][2], (int)times[i][1], (int)times[i][0]),
                    (int)failureTypes[i]
                    ));
            }
            var result = new List<string>();
            foreach (var device in parsedDevices)
                if (device.Date.CompareTo(currentDate) < 0 && Failure.IsFailureSerious(device.FaliureType))
                    result.Add(device.Name);
            return result;
        }
        //public static List<string>  FindDevicesFailedBeforeDate()
        //{

        //}

    }
}
