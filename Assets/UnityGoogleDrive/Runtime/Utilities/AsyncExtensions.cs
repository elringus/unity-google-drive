#if NET_4_6
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace UnityGoogleDrive
{
    public static class AsyncExtensions
    {
        /// <summary>
        /// Allows awaiting <see cref="GoogleDriveRequest"/> objects in async methods.
        /// </summary>
        public static TaskAwaiter<TResponse> GetAwaiter<TResponse> (this GoogleDriveRequestYeildInstruction<TResponse> yeildInstruction)
        {
            var taskCompletionSource = new TaskCompletionSource<TResponse>();
            if (yeildInstruction.GoogleDriveRequest.IsDone) taskCompletionSource.SetResult(yeildInstruction.GoogleDriveRequest.ResponseData);
            else yeildInstruction.OnDone += responseData => taskCompletionSource.SetResult(responseData);
            return taskCompletionSource.Task.GetAwaiter();
        }

    }
}
#endif
