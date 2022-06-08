using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using _3F.Model.Service.Model;

namespace _3F.Model.Service
{
    public interface IActivityListService
    {
        IEnumerable<ActivityModel> GetActivities();
        void AddActivity(ActivityModel activity);
        void ArchiveActivities(int saveActivitiesCount);
    }

    public class ActivityListService : IActivityListService
    {
        private static List<ActivityModel> _activityList;
        private static readonly object LockObj = new object();
        private static string _path;

        public ActivityListService()
        {
            _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "MainActivity.xml");
        }

        public void AddActivity(ActivityModel activity)
        {
            if (activity == null)
                return;

            var oldActivity = GetModel.SingleOrDefault(a => a.User.id == activity.User.id && a.Text == activity.Text);
            if (oldActivity != null)
            {
                oldActivity.Time = activity.Time;
            }
            else
            {
                _activityList.Add(activity);
            }
            _activityList = _activityList.OrderByDescending(a => a.Time).ToList();

            SaveActivities();
        }

        public IEnumerable<ActivityModel> GetActivities()
        {
            return GetModel.Take(10);
        }

        public void ArchiveActivities(int saveActivitiesCount)
        {
            var toArchive = GetModel.Skip(saveActivitiesCount).ToList();
            var date = Info.CentralEuropeNow.ToString("yyyy_MM_dd");
            var archivePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data",
                "MainActivity_" + date + ".xml");

            var serializer = new XmlSerializer(typeof(List<ActivityModel>));
            using (StreamWriter writer = new StreamWriter(archivePath))
            {
                serializer.Serialize(writer, toArchive);
                writer.Flush();
                writer.Close();
            }

            _activityList = GetModel.Take(saveActivitiesCount).ToList();
            SaveActivities();
        }

        private List<ActivityModel> GetModel
        {
            get
            {
                if (_activityList != null)
                    return _activityList;

                lock (LockObj)
                {
                    if (File.Exists(_path))
                    {
                        var serializer = new XmlSerializer(typeof(List<ActivityModel>));
                        using (TextReader reader = new StreamReader(_path))
                        {
                            _activityList = (List<ActivityModel>)serializer.Deserialize(reader);
                        }
                    }
                    else
                    {
                        _activityList = new List<ActivityModel>();
                    }
                }

                return _activityList;
            }
        }

        private void SaveActivities()
        {
            lock (LockObj)
            {
                if (File.Exists(_path))
                {
                    var serializer = new XmlSerializer(typeof (List<ActivityModel>));
                    using (StreamWriter writer = new StreamWriter(_path))
                    {
                        serializer.Serialize(writer, GetModel);
                        writer.Flush();
                        writer.Close();
                    }
                }
            }
        }
    }
}
