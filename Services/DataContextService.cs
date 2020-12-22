using System;
using static Readible.Shared.HttpStatus;

namespace Readible.Shared
{
    public abstract class DataContextService
    {

        protected void CatchTimestampMismatched(DateTime? timestamp1, DateTime? timestamp2)
        {
            if (timestamp1 == null || timestamp2 == null) throw new HttpResponseException(CONFLICT_CODE, TIMESTAMP_MISMATCHED);
            if (Common.CompareTimestamp(timestamp1.Value, timestamp2.Value) == false) throw new HttpResponseException(CONFLICT_CODE, TIMESTAMP_MISMATCHED);
        }

        protected void CatchNotFound(object item, int code = NOT_FOUND_CODE)
        {
            if (item == null) throw new HttpResponseException(code);
        }

        protected void CatchConflict(object item, int code = CONFLICT_CODE)
        {
            if (item != null) throw new HttpResponseException(code);
        }

        protected void CatchCondition(bool condition, int code = BAD_REQUEST_CODE)
        {
            if (condition) throw new HttpResponseException(code);
        }

        protected string UpdateImage(string image, string updateImage, int maxWidth, int maxHeight)
        {
            if (string.IsNullOrEmpty(updateImage)) return image;
            return updateImage == "null" ? null : ImageSharpControl.Resize(updateImage, maxWidth, maxHeight);
        }

        protected string UpdateImage(string updateImage, int maxWidth, int maxHeight)
        {
            return string.IsNullOrEmpty(updateImage) ? null : ImageSharpControl.Resize(updateImage, maxWidth, maxHeight);
        }

    }
}
