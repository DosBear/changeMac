using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Management;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace changeMac
{
    class Program
    {
        static void Main(string[] args)
        {
            "changeMac {0} \r\n".ShowText(Const.App.VERSION);

            "Список найденных интерфейсов:".ShowInfo();
            int cnt = 0;
            Dictionary<int, NetworkInterface> dicNet = new Dictionary<int, NetworkInterface>();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                cnt++;
                "[{0}] {1} MAC {2}".ShowText(cnt.ToString(), item.Description, item.GetPhysicalAddress().ToString());
                dicNet.Add(cnt, item);
            }
            
            "\r\nУкажите номер интерфейса у котороно нужно изменить MAC-адрес:".ShowInfo();
            int current = 0;
            if (int.TryParse(Console.ReadLine(), out current) && current <= cnt && current>=0)
            {
                "Изменеие сведений для {0}. Cтарый адрес {1}".ShowText(dicNet[current].Name, dicNet[current].GetPhysicalAddress().ToString());
                string newMac = GetNewMac();
                "Сгенерирован новый адрес ".ShowKeyValue(newMac, ConsoleColor.Green);
                "\r\nПрописать новый mac-адрес для адаптера Y/N".ShowInfo();
                if (Console.ReadLine().ToUpper() == "Y")
                {
                    try
                    {
                        RegistryKey reg, netkey;
                        new System.Security.Permissions.RegistryPermission(System.Security.Permissions.PermissionState.Unrestricted).Assert();
                        reg = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32);
                        netkey = reg.OpenSubKey(Const.App.REG_MAC, true);
                        foreach (string subKeyName in netkey.GetSubKeyNames())
                        {
                            RegistryKey regnic = reg.OpenSubKey(Const.App.REG_MAC + "\\" + subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree);
                            if (regnic.GetValue("NetCfgInstanceID").ToString() == dicNet[current].Id)
                            {
                                regnic.SetValue("NetworkAddress", newMac);
                                "Mac-адрес {0} успешно прописан для {1}".ShowGreen(newMac, subKeyName);
                                "Выполняется перезагрузка адаптера".ShowText();
                                reloadAdapter(dicNet[current].Description);
                                "Адаптер перзагружен. Сведения обновлены".ShowText();

                                return;
                            }
                        }

                    }
                    catch (Exception err)
                    {
                        "Произошла ошибка: {0}".ShowError(err.Message);
                    }

                }
                else
                {
                    "Операция отменена".ShowText();
                }

            }
            else
            {
                "Ошибка в введенных данных. Операция отменена".ShowError();
            }

        }

        private static string GetNewMac()
        {
            var random = new Random();
            var buffer = new byte[6];
            random.NextBytes(buffer);
            var result = String.Concat(buffer.Select(x => string.Format("{0}", x.ToString("X2"))).ToArray());
            return result;
        }

        private static void reloadAdapter(string adaperName)
        {
            ManagementObject mobj = new ManagementObject();
            ObjectQuery query = new System.Management.ObjectQuery(Const.App.SQL_GETADAPTER.FormatWith(adaperName));
            ManagementObjectSearcher search = new ManagementObjectSearcher(query);
            ManagementObjectCollection coladapters = search.Get();
            foreach (ManagementObject item in coladapters)
            {
                item.InvokeMethod("Disable", null);
                item.InvokeMethod("Enable", null);
            }
        }
    }
}
