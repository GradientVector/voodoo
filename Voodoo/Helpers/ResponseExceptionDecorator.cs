﻿using System;
using Voodoo.Infrastructure;
using Voodoo.Messages;

namespace Voodoo.Helpers
{
    public class ResponseExceptionDecorator
    {
        private readonly IResponse response;
        private Exception exception;

        public ResponseExceptionDecorator(IResponse response, Exception exception)
        {
            this.response = response;
            this.exception = exception;
        }

        public void Decorate()
        {
            var mapper = VoodooGlobalConfiguration.ExceptionTranslator;
            response.IsOk = false;
            var logicException = exception as LogicException;
            if (logicException != null)
            {
                response.HasLogicException = true;
                response.Details = logicException.Details;
                if (logicException.InnerException != null)
                    response.Exception = logicException.InnerException;
            }
            if (mapper.DecorateResponseWithException(exception, response))
                return;

            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                if (mapper.DecorateResponseWithException(exception, response))
                    return;
            }

            response.Message = exception.Message;
            response.Exception = exception;
        }
    }
}