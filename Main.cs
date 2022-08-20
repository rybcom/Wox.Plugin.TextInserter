
using System;
using System.Collections.Generic;
using System.IO;
using Wox.Plugin;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

using mroot_lib;


namespace TextInserter
{
    static class Paths
    {
        public static string EtcFolderPath => mroot_lib.Paths.SystemFolders.etc;
        public static string ConfigFolderPath => Path.Combine(EtcFolderPath, "wox_plugins", "text_inserter");
        public static string ConfigFilePath => Path.Combine(ConfigFolderPath, "config.xml");

    }

    public class Main : IPlugin, IReloadable
    {
        #region members

        private readonly TextMessageManager _messageManager = new TextMessageManager();
        private PluginInitContext _context;

        #endregion

        #region wox overrides

        public void Init(PluginInitContext context)
        {
            _context = context;
            ReloadData();
        }

        public void ReloadData()
        {
            try
            {
                _messageManager.LoadMessages(Paths.ConfigFilePath);

            }
            catch (Exception e)
            {
                _context.API.ShowMsg(e.Message);
            }
        }

        public List<Result> Query(Query query)
        {
            List<Result> resultList = new List<Result>();

            AddTextInsertCommandsFromConfig(resultList, query);
            AddInsertDateCommand(resultList, query);
            AddOpenConfigFolderCommand(resultList, query);

            return resultList;
        }

        #endregion

        #region commands

        private void AddOpenConfigFolderCommand(List<Result> resultList, Query query)
        {
            if (StringTools.IsEqualOnStart(query.Search, "config", "settings"))
            {
                Result command = new Result
                {
                    Title = "Open config folder",
                    Score = 10,
                    IcoPath = "Images\\settings.png",
                    Action = e =>
                    {
                        ProcessStartInfo pinfo = new ProcessStartInfo
                        {
                            FileName = mroot.substitue_enviro_vars("||dcommander||"),
                            Arguments = $"-P L -T {Paths.ConfigFolderPath}"
                        };
                        Process.Start(pinfo);

                        return true;
                    }
                };
                resultList.Add(command);
            }
        }

        private void AddInsertDateCommand(List<Result> resultList, Query query)
        {
            if (StringTools.IsEqualOnStart(query.Search, "date", "datum", "current", "today"))
            {
                Result command = new Result
                {
                    Title = "today's date",
                    Score = 1000,
                    IcoPath = "Images\\text_white.png",
                    Action = e =>
                    {
                        void execution_sending_keys()
                        {
                            string formatted_text =  DateTime.Now.ToString("dd.MM.yyyy");
                            Thread.Sleep(100);
                            SendKeys.SendWait(formatted_text);
                        }
                        new Task(execution_sending_keys).Start();

                        return true;
                    }
                };
                resultList.Add(command);
            }
        }

        private void AddTextInsertCommandsFromConfig(List<Result> resultList, Query query)
        {
            foreach (var item in _messageManager.GetMessages())
            {
                if (StringTools.IsEqualOnStart(query.FirstSearch, item.Key, item.Value))
                {
                    Result commandResult = new Result
                    {
                        Title = item.Key,
                        SubTitle = item.Value,
                        Score = 1000,
                        IcoPath = "Images\\text_white.png",
                        Action = e =>
                        {
                            void execution_sending_keys()
                            {
                                Thread.Sleep(100);
                                SendKeys.SendWait(item.Value);
                            }
                            new Task(execution_sending_keys).Start();

                            return true;
                        }
                    };

                    resultList.Add(commandResult);
                }
            }
        }

        #endregion
    }
}