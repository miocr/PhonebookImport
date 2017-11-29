using System;
using System.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhonebookImportClient.Utils
{

    public class ResourceHelper
    {
        public ResourceManager resourceManager;
    }

    public class StringsResourceHelper : ResourceHelper
    {
        private  string resourceName;
        private string resourceValue;
        
        public StringsResourceHelper(string resName)
        {
            resourceName = resName;
        }

        public string this[int i]
        {
            get
            {
                if (resourceManager == null)
                    resourceManager = new ResourceManager("PhonebookImportClient.Resource.Res", 
                        typeof(ResourceHelper).Assembly);
                resourceValue = resourceManager.GetString(resourceName + (i+1).ToString("0"));
                return resourceValue;
            }
        }
  
    }

    public class ResourceCollection<T>
    {
        // Declare an array to store the data elements.
        private T[] arr = new T[100];

        // Define the indexer to allow client code to use [] notation.
        public T this[int i]
        {
            get { return arr[i]; }
            set { arr[i] = value; }
        }
    }



}
