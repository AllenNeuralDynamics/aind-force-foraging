//----------------------
// <auto-generated>
//     Generated using the NJsonSchema v10.9.0.0 (Newtonsoft.Json v13.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------


namespace AindForceForagingDataSchema.Session
{
    #pragma warning disable // Disable all warnings

    [System.CodeDom.Compiler.GeneratedCodeAttribute("Bonsai.Sgen", "0.3.0.0 (Newtonsoft.Json v13.0.0.0)")]
    [Bonsai.CombinatorAttribute()]
    [Bonsai.WorkflowElementCategoryAttribute(Bonsai.ElementCategory.Source)]
    public partial class AindBehaviorSessionModel
    {
    
        private string _aindBehaviorServicesPkgVersion = "0.8.5";
    
        private string _version = "0.3.0";
    
        private string _experiment;
    
        private System.Collections.Generic.List<string> _experimenter = new System.Collections.Generic.List<string>();
    
        private System.DateTimeOffset _date;
    
        private string _rootPath;
    
        private string _sessionName;
    
        private string _subject;
    
        private string _experimentVersion;
    
        private string _notes;
    
        private string _commitHash;
    
        private bool _allowDirtyRepo = false;
    
        private bool _skipHardwareValidation = false;
    
        public AindBehaviorSessionModel()
        {
        }
    
        protected AindBehaviorSessionModel(AindBehaviorSessionModel other)
        {
            _aindBehaviorServicesPkgVersion = other._aindBehaviorServicesPkgVersion;
            _version = other._version;
            _experiment = other._experiment;
            _experimenter = other._experimenter;
            _date = other._date;
            _rootPath = other._rootPath;
            _sessionName = other._sessionName;
            _subject = other._subject;
            _experimentVersion = other._experimentVersion;
            _notes = other._notes;
            _commitHash = other._commitHash;
            _allowDirtyRepo = other._allowDirtyRepo;
            _skipHardwareValidation = other._skipHardwareValidation;
        }
    
        [Newtonsoft.Json.JsonPropertyAttribute("aind_behavior_services_pkg_version")]
        public string AindBehaviorServicesPkgVersion
        {
            get
            {
                return _aindBehaviorServicesPkgVersion;
            }
            set
            {
                _aindBehaviorServicesPkgVersion = value;
            }
        }
    
        [Newtonsoft.Json.JsonPropertyAttribute("version")]
        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }
    
        /// <summary>
        /// Name of the experiment
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("experiment", Required=Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DescriptionAttribute("Name of the experiment")]
        public string Experiment
        {
            get
            {
                return _experiment;
            }
            set
            {
                _experiment = value;
            }
        }
    
        /// <summary>
        /// Name of the experimenter
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [Newtonsoft.Json.JsonPropertyAttribute("experimenter")]
        [System.ComponentModel.DescriptionAttribute("Name of the experimenter")]
        public System.Collections.Generic.List<string> Experimenter
        {
            get
            {
                return _experimenter;
            }
            set
            {
                _experimenter = value;
            }
        }
    
        /// <summary>
        /// Date of the experiment
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [Newtonsoft.Json.JsonPropertyAttribute("date")]
        [System.ComponentModel.DescriptionAttribute("Date of the experiment")]
        public System.DateTimeOffset Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }
    
        /// <summary>
        /// Root path where data will be logged
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("root_path", Required=Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DescriptionAttribute("Root path where data will be logged")]
        public string RootPath
        {
            get
            {
                return _rootPath;
            }
            set
            {
                _rootPath = value;
            }
        }
    
        /// <summary>
        /// Name of the session. This will be used to create a folder in the root path.
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("session_name")]
        [System.ComponentModel.DescriptionAttribute("Name of the session. This will be used to create a folder in the root path.")]
        public string SessionName
        {
            get
            {
                return _sessionName;
            }
            set
            {
                _sessionName = value;
            }
        }
    
        /// <summary>
        /// Name of the subject
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("subject", Required=Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DescriptionAttribute("Name of the subject")]
        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
            }
        }
    
        /// <summary>
        /// Version of the experiment
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("experiment_version", Required=Newtonsoft.Json.Required.Always)]
        [System.ComponentModel.DescriptionAttribute("Version of the experiment")]
        public string ExperimentVersion
        {
            get
            {
                return _experimentVersion;
            }
            set
            {
                _experimentVersion = value;
            }
        }
    
        /// <summary>
        /// Notes about the experiment
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("notes")]
        [System.ComponentModel.DescriptionAttribute("Notes about the experiment")]
        public string Notes
        {
            get
            {
                return _notes;
            }
            set
            {
                _notes = value;
            }
        }
    
        /// <summary>
        /// Commit hash of the repository
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("commit_hash")]
        [System.ComponentModel.DescriptionAttribute("Commit hash of the repository")]
        public string CommitHash
        {
            get
            {
                return _commitHash;
            }
            set
            {
                _commitHash = value;
            }
        }
    
        /// <summary>
        /// Allow running from a dirty repository
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("allow_dirty_repo")]
        [System.ComponentModel.DescriptionAttribute("Allow running from a dirty repository")]
        public bool AllowDirtyRepo
        {
            get
            {
                return _allowDirtyRepo;
            }
            set
            {
                _allowDirtyRepo = value;
            }
        }
    
        /// <summary>
        /// Skip hardware validation
        /// </summary>
        [Newtonsoft.Json.JsonPropertyAttribute("skip_hardware_validation")]
        [System.ComponentModel.DescriptionAttribute("Skip hardware validation")]
        public bool SkipHardwareValidation
        {
            get
            {
                return _skipHardwareValidation;
            }
            set
            {
                _skipHardwareValidation = value;
            }
        }
    
        public System.IObservable<AindBehaviorSessionModel> Process()
        {
            return System.Reactive.Linq.Observable.Defer(() => System.Reactive.Linq.Observable.Return(new AindBehaviorSessionModel(this)));
        }
    
        public System.IObservable<AindBehaviorSessionModel> Process<TSource>(System.IObservable<TSource> source)
        {
            return System.Reactive.Linq.Observable.Select(source, _ => new AindBehaviorSessionModel(this));
        }
    
        protected virtual bool PrintMembers(System.Text.StringBuilder stringBuilder)
        {
            stringBuilder.Append("aind_behavior_services_pkg_version = " + _aindBehaviorServicesPkgVersion + ", ");
            stringBuilder.Append("version = " + _version + ", ");
            stringBuilder.Append("experiment = " + _experiment + ", ");
            stringBuilder.Append("experimenter = " + _experimenter + ", ");
            stringBuilder.Append("date = " + _date + ", ");
            stringBuilder.Append("root_path = " + _rootPath + ", ");
            stringBuilder.Append("session_name = " + _sessionName + ", ");
            stringBuilder.Append("subject = " + _subject + ", ");
            stringBuilder.Append("experiment_version = " + _experimentVersion + ", ");
            stringBuilder.Append("notes = " + _notes + ", ");
            stringBuilder.Append("commit_hash = " + _commitHash + ", ");
            stringBuilder.Append("allow_dirty_repo = " + _allowDirtyRepo + ", ");
            stringBuilder.Append("skip_hardware_validation = " + _skipHardwareValidation);
            return true;
        }
    
        public override string ToString()
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append(GetType().Name);
            stringBuilder.Append(" { ");
            if (PrintMembers(stringBuilder))
            {
                stringBuilder.Append(" ");
            }
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }
    }


    /// <summary>
    /// Serializes a sequence of data model objects into JSON strings.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Bonsai.Sgen", "0.3.0.0 (Newtonsoft.Json v13.0.0.0)")]
    [System.ComponentModel.DescriptionAttribute("Serializes a sequence of data model objects into JSON strings.")]
    [Bonsai.CombinatorAttribute()]
    [Bonsai.WorkflowElementCategoryAttribute(Bonsai.ElementCategory.Transform)]
    public partial class SerializeToJson
    {
    
        private System.IObservable<string> Process<T>(System.IObservable<T> source)
        {
            return System.Reactive.Linq.Observable.Select(source, value => Newtonsoft.Json.JsonConvert.SerializeObject(value));
        }

        public System.IObservable<string> Process(System.IObservable<AindBehaviorSessionModel> source)
        {
            return Process<AindBehaviorSessionModel>(source);
        }
    }


    /// <summary>
    /// Deserializes a sequence of JSON strings into data model objects.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Bonsai.Sgen", "0.3.0.0 (Newtonsoft.Json v13.0.0.0)")]
    [System.ComponentModel.DescriptionAttribute("Deserializes a sequence of JSON strings into data model objects.")]
    [System.ComponentModel.DefaultPropertyAttribute("Type")]
    [Bonsai.WorkflowElementCategoryAttribute(Bonsai.ElementCategory.Transform)]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Bonsai.Expressions.TypeMapping<AindBehaviorSessionModel>))]
    public partial class DeserializeFromJson : Bonsai.Expressions.SingleArgumentExpressionBuilder
    {
    
        public DeserializeFromJson()
        {
            Type = new Bonsai.Expressions.TypeMapping<AindBehaviorSessionModel>();
        }

        public Bonsai.Expressions.TypeMapping Type { get; set; }

        public override System.Linq.Expressions.Expression Build(System.Collections.Generic.IEnumerable<System.Linq.Expressions.Expression> arguments)
        {
            var typeMapping = (Bonsai.Expressions.TypeMapping)Type;
            var returnType = typeMapping.GetType().GetGenericArguments()[0];
            return System.Linq.Expressions.Expression.Call(
                typeof(DeserializeFromJson),
                "Process",
                new System.Type[] { returnType },
                System.Linq.Enumerable.Single(arguments));
        }

        private static System.IObservable<T> Process<T>(System.IObservable<string> source)
        {
            return System.Reactive.Linq.Observable.Select(source, value => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value));
        }
    }
}