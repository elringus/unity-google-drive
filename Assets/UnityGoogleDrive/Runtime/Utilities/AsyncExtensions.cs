using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace UnityGoogleDrive
{
    public static class AsyncExtensions
    {
        /// <summary>
        /// Allows awaiting <see cref="GoogleDriveRequest"/> objects in async methods.
        /// </summary>
        public static TaskAwaiter<TResponse> GetAwaiter<TResponse> (this GoogleDriveRequestYieldInstruction<TResponse> yieldInstruction)
        {
            var taskCompletionSource = new TaskCompletionSource<TResponse>();
            if (yieldInstruction.GoogleDriveRequest.IsDone) taskCompletionSource.SetResult(yieldInstruction.GoogleDriveRequest.ResponseData);
            else yieldInstruction.OnDone += responseData => taskCompletionSource.SetResult(responseData);
            return taskCompletionSource.Task.GetAwaiter();
        }

        public static TaskAwaiter GetAwaiter (this GoogleDriveRequestYieldInstruction yieldInstruction)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            if (yieldInstruction.IsDone) taskCompletionSource.SetResult(null);
            else yieldInstruction.OnDoneNonGeneric += () => taskCompletionSource.SetResult(null);
            return (taskCompletionSource.Task as Task).GetAwaiter();
        }
    }
}
