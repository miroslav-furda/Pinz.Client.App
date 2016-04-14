using System.ServiceModel;

namespace Com.Pinz.Client.RemoteServiceConsumer.ServiceImpl
{
    abstract class ServiceBase
    {
        protected void CloseOrAbortServiceChannel(ICommunicationObject communicationObject)
        {
            if (communicationObject == null || communicationObject.State == CommunicationState.Closed)
            {
                return;
            }

            try
            {
                if (communicationObject.State != CommunicationState.Faulted)
                {
                    communicationObject.Close();
                }
            }
            finally
            {
                if (communicationObject.State != CommunicationState.Closed)
                {
                    communicationObject.Abort();
                }
            }
        }

        abstract public void OpenChannel();
        abstract public void CloseChannel();
    }
}
