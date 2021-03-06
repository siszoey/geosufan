using ESRI.ArcGIS.Geodatabase;
namespace GeoDBIntegration
{
    public class CopyPasteGDBData
    {



        public static void CopyPasteGeodatabaseData(IWorkspace sourceWorkspace, IWorkspace targetWorkspace, string objectName, esriDatasetType esriDataType)
        {
            // Validate input

            if ((sourceWorkspace.Type == esriWorkspaceType.esriFileSystemWorkspace) || (targetWorkspace.Type == esriWorkspaceType.esriFileSystemWorkspace))
            {
                return; // Should be a throw
            }
            //create source workspace name

            IDataset sourceWorkspaceDataset = (IDataset)sourceWorkspace;

            IWorkspaceName sourceWorkspaceName = (IWorkspaceName)sourceWorkspaceDataset.FullName;

            //create target workspace name

            IDataset targetWorkspaceDataset = (IDataset)targetWorkspace;

            IWorkspaceName targetWorkspaceName = (IWorkspaceName)targetWorkspaceDataset.FullName;

            //Create Name Object Based on data type

            IDatasetName datasetName;

            switch (esriDataType)
            {
                case esriDatasetType.esriDTFeatureDataset:

                    IFeatureDatasetName inFeatureDatasetName = new FeatureDatasetNameClass();

                    datasetName = (IDatasetName)inFeatureDatasetName;
                    break;
                case esriDatasetType.esriDTFeatureClass:

                    IFeatureClassName inFeatureClassName = new FeatureClassNameClass();

                    datasetName = (IDatasetName)inFeatureClassName;

                    break;
                case esriDatasetType.esriDTTable:

                    ITableName inTableName = new TableNameClass();

                    datasetName = (IDatasetName)inTableName;

                    break;

                case esriDatasetType.esriDTGeometricNetwork:

                    IGeometricNetworkName inGeometricNetworkName = new GeometricNetworkNameClass();

                    datasetName = (IDatasetName)inGeometricNetworkName;

                    break;

                case esriDatasetType.esriDTRelationshipClass:

                    IRelationshipClassName inRelationshipClassName = new RelationshipClassNameClass();

                    datasetName = (IDatasetName)inRelationshipClassName;
                    break;

                case esriDatasetType.esriDTNetworkDataset:

                    INetworkDatasetName inNetworkDatasetName = new NetworkDatasetNameClass();

                    datasetName = (IDatasetName)inNetworkDatasetName;

                    break;

                case esriDatasetType.esriDTTopology:

                    ITopologyName inTopologyName = new TopologyNameClass();

                    datasetName = (IDatasetName)inTopologyName;

                    break;

                default:

                    return; // Should be a throw
            }

            // Set the name of the object to be copied

            datasetName.WorkspaceName = (IWorkspaceName)sourceWorkspaceName;

            datasetName.Name = objectName;

            //Setup mapping for copy/paste

            IEnumNameMapping fromNameMapping;

            ESRI.ArcGIS.esriSystem.IEnumNameEdit editFromName;

            ESRI.ArcGIS.esriSystem.IEnumName fromName = new NamesEnumerator();

            editFromName = (ESRI.ArcGIS.esriSystem.IEnumNameEdit)fromName;

            editFromName.Add((ESRI.ArcGIS.esriSystem.IName)datasetName);

            ESRI.ArcGIS.esriSystem.IName toName = (ESRI.ArcGIS.esriSystem.IName)targetWorkspaceName;

            // Generate name mapping


            ESRI.ArcGIS.Geodatabase.IGeoDBDataTransfer geoDBDataTransfer = new GeoDBDataTransferClass();

            bool targetWorkspaceExists;

            targetWorkspaceExists = geoDBDataTransfer.GenerateNameMapping(fromName, toName, out fromNameMapping);

            // Copy/Pate the data

            geoDBDataTransfer.Transfer(fromNameMapping, toName);


        }

    }
}

