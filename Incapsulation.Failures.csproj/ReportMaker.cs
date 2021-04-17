using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.Failures
{
    public enum FailureType
    {
        US, NR, HF, CP
    }

    public class Device
    {
        public int ID { get; private set; }
        public string Name { get; private set; }


        public Device(int id, string name)
        {
            this.ID = id;
            this.Name = name;
        }
    }

    public class Failure
    {
        public bool IsFailureSerious()=> FailureType == FailureType.US || FailureType == FailureType.HF; 
        
        public DateTime Date { get; private set; }
        public FailureType FailureType { get; private set; }
        public Device Device { get; private set; }

        public Failure(Device device, FailureType failure, DateTime failureDate)
        {
            this.Date = failureDate;
            this.Device = device;
            this.FailureType = failure;
        }
    }

    public class ReportMaker
    {
        public Failure Failure
        {
            get => default;
            set
            {
            }
        }

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
            List<Failure> parsedFailurse = new List<Failure>();
            for(int i = 0; i != devices.Count;i++)
            {
                parsedFailurse.Add(
                    new Failure(
                        new Device((int)devices[i]["DeviceId"],devices[i]["Name"].ToString()),
                        (FailureType)failureTypes[i],
                        new DateTime((int)times[i][2], (int)times[i][1], (int)times[i][0])));
            }
            var result = new List<string>();
            foreach (var failure in parsedFailurse)
                if (failure.Date.CompareTo(currentDate) < 0 && failure.IsFailureSerious())
                    result.Add(failure.Device.Name);
            return result;
        }

        public static List<string> FindDevicesFailedBeforeDate(Failure[] failures,DateTime currentDate)
        {
            List<string> result = new List<string>();
            foreach (var failure in failures)
                if (failure.Date.CompareTo(currentDate) < 0 && failure.IsFailureSerious() )
                    result.Add(failure.Device.Name);
            return result;
        }
    }
}
