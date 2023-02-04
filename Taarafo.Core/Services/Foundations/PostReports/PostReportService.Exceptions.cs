﻿// ---------------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE TO CONNECT THE WORLD
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Taarafo.Core.Models.PostReports;
using Taarafo.Core.Models.PostReports.Exceptions;
using Xeptions;

namespace Taarafo.Core.Services.Foundations.PostReports
{
    public partial class PostReportService
    {
        private delegate ValueTask<PostReport> ReturningPostReportFunction();

        private async ValueTask<PostReport> TryCatch(ReturningPostReportFunction returningPostReportFunction)
        {
            try
            {
                return await returningPostReportFunction();
            }
            catch (NullPostReportException nullPostReportException)
            {
                throw CreateAndLogValidationException(nullPostReportException);
            }
            catch (InvalidPostReportException invalidPostReportException)
            {
                throw CreateAndLogValidationException(invalidPostReportException);
            }
            catch (SqlException sqlException)
            {
                var failedPostReportStorageException = new FailedPostReportStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedPostReportStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsPostReportException =
                    new AlreadyExistsPostReportException(duplicateKeyException);

                throw CreateAndDependencyValidationException(alreadyExistsPostReportException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedPostReportException =
                    new LockedPostReportException(dbUpdateConcurrencyException);

                throw CreateAndDependencyValidationException(lockedPostReportException);
            }
            catch (Exception exception)
            {
                var postReportServiceException =
                    new FailedPostReportServiceException(exception);

                throw CreateAndLogServiceException(postReportServiceException);
            }
        }

        private PostReportValidationException CreateAndLogValidationException(Xeption exception)
        {
            var postReportValidationException =
                new PostReportValidationException(exception);

            this.loggingBroker.LogError(postReportValidationException);

            return postReportValidationException;
        }

        private PostReportDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var postReportDependencyException = new PostReportDependencyException(exception);
            this.loggingBroker.LogCritical(postReportDependencyException);

            return postReportDependencyException;
        }

        private PostReportDependencyValidationException CreateAndDependencyValidationException(Xeption exception)
        {
            var postReportDependencyValidationException =
                new PostReportDependencyValidationException(exception);

            this.loggingBroker.LogError(postReportDependencyValidationException);

            return postReportDependencyValidationException;
        }

        private PostReportServiceException CreateAndLogServiceException(Xeption exception)
        {
            var postReportServiceException = new PostReportServiceException(exception);
            this.loggingBroker.LogError(postReportServiceException);

            return postReportServiceException;
        }
    }
}