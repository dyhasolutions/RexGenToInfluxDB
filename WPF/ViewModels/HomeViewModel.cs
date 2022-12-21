using DAL.Data.UnitOfWork;
using DAL.InfluxDBService;
using DAL.VehicleServerService;
using Microsoft.Win32;
using MODELS.VehicleServerInfo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPF.Views;
using DbcParserLib;
using DbcParserLib.Influx;
using System.IO;
using InfluxShared;
using MODELS;
using InfluxShared.FileObjects;
using RXD.Base;
using DAL;

namespace WPF.ViewModels
{
    public class HomeViewModel : MainViewModel
    {
        InfluxDBHelper InfluxDBHelper = new InfluxDBHelper();

        #region constructor
        public HomeViewModel()
        {
            ServerTypeInfluxDB = unitOfWork.ServerTypeRepo
                .Get()
                .Where(x => x.Name == "InfluxDB")
                .FirstOrDefault();
            RefreshFields(1);
        }
        #endregion

        #region implementation baseViewModel
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            switch (parameter.ToString().ToLower())
            {
                case "selectrxdfiles": ImportRXDFiles(); break;
                case "selectdbcfile": ImportDBCFile(); break;
                case "newvehicle":
                    CreateVehicleVisibility = Visibility.Visible;
                    CreateServerVisibility = Visibility.Collapsed;
                        ; break;
                case "createnewvehicle": CreateNewVehicle(); break;
                case "newserver":
                    CreateVehicleVisibility = Visibility.Collapsed;
                    CreateServerVisibility = Visibility.Visible; ; 
                    break;
                case "createnewserver": CreateNewServer(); break;
                case "export": ExportRXDFilesToInfluxDB(); break;

                default:
                    break;
            }
        }


        public override string this[string columnName]
        {
            get
            {
                if (columnName == "Manufacturer" && string.IsNullOrEmpty(Manufacturer))
                {
                    return "Manufacturer is mandetory!";
                }
                if (columnName == "Model" && string.IsNullOrEmpty(Model))
                {
                    return "Model is mandetory!";
                }
                if (columnName == "Type" && string.IsNullOrEmpty(Type))
                {
                    return "Type is mandetory!";
                }
                if (columnName == "VIN" && string.IsNullOrEmpty(VIN))
                {
                    return "VIN is mandetory!";
                }
                if (columnName == "Name" && string.IsNullOrEmpty(Name))
                {
                    return "Server alias is mandetory!";
                }
                if (columnName == "IP_URL" && string.IsNullOrEmpty(IP_URL))
                {
                    return "An IP adress or URL is mandetory!";
                }
                if (columnName == "Token" && string.IsNullOrEmpty(Token))
                {
                    return "A token is mandetory!";
                }

                return "";
            }
        }
        #endregion

        #region attributes
        private IUnitOfWork unitOfWork = new UnitOfWork(new VehicleServerContext());
        private string _inputPathRXDFile;
        private ObservableCollection<string> _rxdFiles;
        private string _inputPathDBCFile;
        private string _exportingStatus;
        private ObservableCollection<Vehicle> _vehicles;
        private ObservableCollection<Server> _servers;

        #region vehicle attributes
        private string _manufacturer;
        private string _model;
        private string _type;
        private string _vin;
        private Visibility _createVehicleVisibility;
        private Vehicle _selectedVehicle;
        #endregion

        #region InfluxDB server attributes
        private Visibility _createServerVisibility;
        private string _ip_URL;
        private string _name;
        private string _token;
        private ServerType _serverType;
        private Server _selectedServer;


        #endregion

        #endregion

        #region properties
        public string InputPathRXDFile
        {
            get { return _inputPathRXDFile; }
            set { _inputPathRXDFile = value; }
        }
        public ObservableCollection<string> RXDFiles
        {
            get { return _rxdFiles; }
            set { _rxdFiles = value;  NotifyPropertyChanged(); }
        }
        public string InputPathDBCFile
        {
            get { return _inputPathDBCFile; }
            set { _inputPathDBCFile = value; }
        }
        public string ExportingStatus
        {
            get { return _exportingStatus; }
            set { _exportingStatus = value; }
        }
        public ObservableCollection<Vehicle> Vehicles
        {
            get { return _vehicles; }
            set { _vehicles = value; NotifyPropertyChanged(); }
        }
        public ObservableCollection<Server> Servers
        {
            get { return _servers; }
            set { _servers = value; NotifyPropertyChanged(); }
        }

        #region vehicle properties
        public Visibility CreateVehicleVisibility
        {
            get { return _createVehicleVisibility; }
            set { _createVehicleVisibility = value; NotifyPropertyChanged(); }
        }

        public string Manufacturer
        {
            get { return _manufacturer; }
            set { _manufacturer = value; }
        }
        public string VIN
        {
            get { return _vin; }
            set { _vin = value; }
        }
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }
        public Vehicle SelectedVehicle
        {
            get { return _selectedVehicle; }
            set { _selectedVehicle = value; NotifyPropertyChanged(); }
        }
        #endregion

        #region InfluxDB server properties
        public Visibility CreateServerVisibility
        {
            get { return _createServerVisibility; }
            set { _createServerVisibility = value; NotifyPropertyChanged(); }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string IP_URL
        {
            get { return _ip_URL; }
            set { _ip_URL = value; }
        }
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }
        public ServerType ServerTypeInfluxDB
        {
            get { return _serverType; }
            set { _serverType = value; }
        }
        public Server SelectedServer
        {
            get { return _selectedServer; }
            set { _selectedServer = value; NotifyPropertyChanged(); }
        }
        #endregion

        #endregion

        #region basic crud operations
        public void CreateNewVehicle()
        {
            Name = " ";
            IP_URL = " ";
            Token = " ";

            ExportingStatus = "Creating new vehicle.";

            if (this.IsValid())
            {
                Vehicle newVehicle = new Vehicle()
                {
                    Manufacturer = this.Manufacturer,
                    Model = this.Model,
                    Type = this.Type,
                    VIN = this.VIN
                };
                unitOfWork.VehicleRepo.Add(newVehicle);
                int ok = unitOfWork.Save();
                RefreshFieldsAfterCreatedVehicle(ok);
                ExportingStatus = (ok == 1) ? "New vehicle created." : "Check form for faults!";
            }
        }
        public void CreateNewServer()
        {
            ExportingStatus = "Creating new InfluxDb server.";

            Manufacturer = " ";
            Model = " ";
            Type = " ";
            VIN = " ";

            if (this.IsValid())
            {
                ServerType InfluxDBserverType = new ServerType();
                InfluxDBserverType = unitOfWork.ServerTypeRepo
                    .Get()
                    .Where(x => x.Name == "InfluxDB")
                    .FirstOrDefault();
                if (InfluxDBserverType == null)
                {
                    ServerType newInfluxDBServerType = new ServerType()
                    {
                        Name = "InfluxDB"
                    };
                    unitOfWork.ServerTypeRepo.Add(newInfluxDBServerType);
                    int ok1 = unitOfWork.Save();
                    if (ok1 == 1)
                    {
                        InfluxDBserverType = unitOfWork.ServerTypeRepo
                            .Get()
                            .Where(x => x.Name == "InfluxDB")
                            .FirstOrDefault();
                    }
                }

                ServerCredentials serverCredentials = new ServerCredentials();
                serverCredentials = unitOfWork.ServerCredentialsRepo
                    .Get()
                    .Where(x => x.Token == Token)
                    .FirstOrDefault();
                if (serverCredentials == null)
                {
                    ServerCredentials newServerCredentials = new ServerCredentials()
                    {
                        Token = Token
                    };
                    unitOfWork.ServerCredentialsRepo.Add(newServerCredentials);
                    int ok2 = unitOfWork.Save();
                    if (ok2 == 1)
                    {
                        serverCredentials = unitOfWork.ServerCredentialsRepo
                            .Get()
                            .Where(x => x.Token == Token)
                            .FirstOrDefault();
                    }
                }

                Server newServer = new Server()
                {
                    IP_URL = IP_URL,
                    Name = Name,
                    ServerTypeID = InfluxDBserverType.ID,
                    ServerCredentialsID = serverCredentials.ID
                };
                unitOfWork.ServerRepo.Add(newServer);
                int ok = unitOfWork.Save();
                RefreshFieldsAfterCreatedServer(ok);
                ExportingStatus = (ok == 1) ? "New InfluxDB server created." : "Check form for faults!";
            }
        }
        #endregion

        #region extra methods
        private void ImportRXDFiles()
        {
            RXDFiles = new ObservableCollection<string>();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "RXD Files (*.rxd)|*.rxd";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (ofd.ShowDialog() == true)
            {
                foreach (string inputFile in ofd.FileNames)
                {
                    RXDFiles.Add(inputFile);
                }
            }
        }
        private void ImportDBCFile()
        {
            InputPathDBCFile = "";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "DBC File (*.dbc)|*.dbc";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (ofd.ShowDialog() == true)
            {
                InputPathDBCFile = ofd.FileName;
            }
        }
        private void RefreshFields(int ok)
        {
            if (ok == 1)
            {
                ExportingStatus = null;

                RXDFiles = null;
                InputPathDBCFile = null;
                Vehicles = new ObservableCollection<Vehicle>(unitOfWork.VehicleRepo.Get());
                Servers = new ObservableCollection<Server>(unitOfWork.ServerRepo.Get()
                    .Where(x => x.ServerTypeID == ServerTypeInfluxDB.ID));

                Manufacturer = "";
                Model = "";
                Type = "";
                VIN = "";
                CreateVehicleVisibility = Visibility.Collapsed;

                CreateServerVisibility = Visibility.Collapsed;
            }
        }
        private void RefreshFieldsAfterCreatedVehicle(int ok)
        {
            if (ok == 1)
            {
                Vehicles = new ObservableCollection<Vehicle>(unitOfWork.VehicleRepo.Get());

                Manufacturer = "";
                Model = "";
                Type = "";
                VIN = "";
                CreateVehicleVisibility = Visibility.Collapsed;

                Name = "";
                IP_URL = "";
                Token = "";
            }
        }
        private void RefreshFieldsAfterCreatedServer(int ok)
        {
            if (ok == 1)
            {
                ServerTypeInfluxDB = unitOfWork.ServerTypeRepo
                .Get()
                .Where(x => x.Name == "InfluxDB")
                .FirstOrDefault();
                Servers = new ObservableCollection<Server>(unitOfWork.ServerRepo
                    .Get()
                    .Where(x => x.ServerTypeID == ServerTypeInfluxDB.ID));

                Name = "";
                IP_URL = "";
                Token = "";
                CreateServerVisibility = Visibility.Collapsed;

                Manufacturer = "";
                Model = "";
                Type = "";
                VIN = "";
            }
        }
        private void ExportRXDFilesToInfluxDB()
        {
            CreateVehicleVisibility = Visibility.Collapsed;
            CreateServerVisibility = Visibility.Collapsed;
            ExportingStatus = "";

            //DBC logic
            Stream dbcStream = new MemoryStream(File.ReadAllBytes(InputPathDBCFile));
            Dbc dbc = Parser.ParseFromStream(dbcStream);
            DBC influxDBC = (DbcToInfluxObj.FromDBC(dbc) as DBC);
            ExportDbcCollection signalsCollection = DbcToInfluxObj.LoadExportSignalsFromDBC(influxDBC);

            MemoryStream outStream = new MemoryStream();
            List<TimestampData> timestampDatas = new List<TimestampData>();

            #region form checks
            if (RXDFiles == null)
            {
                ExportingStatus = "Select the necesarry RXD file(s)." + Environment.NewLine;
            };
            if (InputPathDBCFile == null)
            {
                ExportingStatus += "Select a DBC file." + Environment.NewLine;
            }
            if (SelectedVehicle == null)
            {
                ExportingStatus += "Select a vehicle." + Environment.NewLine;
            }
            else
            {
                SelectedVehicle = unitOfWork.VehicleRepo
                    .Get()
                    .Where(x => x.ID == SelectedVehicle.ID)
                    .FirstOrDefault();
            }
            if (SelectedServer == null)
            {
                ExportingStatus += "Select an InfluxDB server.";
            }
            else
            {
                SelectedServer = unitOfWork.ServerRepo
                    .Get()
                    .Where(x => x.ID == SelectedServer.ID)
                    .FirstOrDefault();
            }
            #endregion

            try
            {
                foreach (string rxdFile in RXDFiles)
                {
                    Stream rxdStream = new MemoryStream(File.ReadAllBytes(rxdFile));
                    string filename = Path.GetFileName(rxdFile);

                    using (BinRXD rxd = BinRXD.Load($"http://www.test.com/RexGen {filename}", rxdStream))
                        if (rxd is not null)
                        {
                            timestampDatas = rxd.ExportToCustomObjects(new BinRXD.ExportSettings()
                            {
                                StorageCache = StorageCacheType.Memory,
                                SignalsDatabase = new()
                                {
                                    dbcCollection = signalsCollection
                                }
                            }
                            );
                        };
                    //InfluxDBHelper.WriteToInfluxDB(timestampDatas, test);

                    //using (FileStream fs = new FileStream("C:/Users/dylan/Desktop/test2.csv", FileMode.Create, System.IO.FileAccess.Write))
                    //    DataHelper.Convert(rxd, new BinRXD.ExportSettings()
                    //    {
                    //        StorageCache = StorageCacheType.Memory,
                    //        SignalsDatabase = new() { dbcCollection = signalsCollection },
                    //    }, fs, "csv:influxdb");
                }
            }
            catch (Exception ex)
            {
                ExportingStatus = ex.ToString();
            }

        }
        #endregion
    }
}
