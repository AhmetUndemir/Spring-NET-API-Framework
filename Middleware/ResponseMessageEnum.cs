using System.ComponentModel;

namespace SimpleSprint.Middleware {
    public enum ResponseMessageEnum {

        [Description ("OK")]
        Success, 
        
        [Description ("Request responded with exceptions.")]
        Exception, 
        
        [Description ("Request denied.")]
        UnAuthorized, 
        
        [Description ("Request responded with validation error(s).")]
        ValidationError, 
        
        [Description ("Unable to process the request.")]
        Failure
    }
}