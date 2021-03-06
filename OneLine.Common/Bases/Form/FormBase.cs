﻿using FluentValidation;
using Microsoft.Extensions.Configuration;
using OneLine.Enums;
using OneLine.Extensions;
using OneLine.Models;
using OneLine.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneLine.Bases
{
    public class FormBase<T, TIdentifier, TId, THttpService, TBlobData, TBlobValidator, TUserBlobs> : 
        IForm<T, TIdentifier, THttpService, TBlobData, TBlobValidator, TUserBlobs>
        where T : new()
        where TIdentifier : IIdentifier<TId>, new()
        where TId : class
        where THttpService : HttpBaseCrudExtendedService<T, TIdentifier, TId, TBlobData, TBlobValidator, TUserBlobs>, new()
        where TBlobData : IBlobData
        where TBlobValidator : IValidator, new()
        where TUserBlobs : IUserBlobs
    {
        public virtual T Record { get; set; } = new T();
        public virtual IEnumerable<T> Records { get; set; }
        public virtual TIdentifier Identifier { get; set; } = new TIdentifier();
        public virtual THttpService HttpService { get; set; } = new THttpService();
        public virtual IList<TBlobData> BlobDatas { get; set; } = new List<TBlobData>();
        public virtual IConfiguration Configuration { get; set; }
        public virtual FormState FormState { get; set; }
        public virtual Action<IResponseResult<IApiResponse<T>>> OnLoad { get; set; }
        public virtual Action<IResponseResult<IApiResponse<T>>> OnLoadSucceeded { get; set; }
        public virtual Action<IResponseResult<IApiResponse<T>>> OnLoadException { get; set; }
        public virtual Action<IResponseResult<IApiResponse<T>>> OnLoadFailed { get; set; }
        public virtual IResponseResult<IApiResponse<T>> Response { get; set; }
        public virtual Action<IResponseResult<IApiResponse<T>>> OnResponse { get; set; }
        public virtual Action<IResponseResult<IApiResponse<T>>> OnResponseSucceeded { get; set; }
        public virtual Action<IResponseResult<IApiResponse<T>>> OnResponseException { get; set; }
        public virtual Action<IResponseResult<IApiResponse<T>>> OnResponseFailed { get; set; }
        public virtual IResponseResult<IApiResponse<Tuple<T, IEnumerable<TUserBlobs>>>> ResponseAddWithBlobs { get; set; }
        public virtual Action<IResponseResult<IApiResponse<Tuple<T, IEnumerable<TUserBlobs>>>>> OnResponseAddWithBlobs { get; set; }
        public virtual Action<IResponseResult<IApiResponse<Tuple<T, IEnumerable<TUserBlobs>>>>> OnResponseAddWithBlobsSucceeded { get; set; }
        public virtual Action<IResponseResult<IApiResponse<Tuple<T, IEnumerable<TUserBlobs>>>>> OnResponseAddWithBlobsException { get; set; }
        public virtual Action<IResponseResult<IApiResponse<Tuple<T, IEnumerable<TUserBlobs>>>>> OnResponseAddWithBlobsFailed { get; set; }
        public virtual IResponseResult<IApiResponse<Tuple<T, IEnumerable<TUserBlobs>, IEnumerable<TUserBlobs>>>> ResponseUpdateWithBlobs { get; set; }
        public virtual Action<IResponseResult<IApiResponse<Tuple<T, IEnumerable<TUserBlobs>, IEnumerable<TUserBlobs>>>>> OnResponseUpdateWithBlobs { get; set; }
        public virtual Action<IResponseResult<IApiResponse<Tuple<T, IEnumerable<TUserBlobs>, IEnumerable<TUserBlobs>>>>> OnResponseUpdateWithBlobsSucceeded { get; set; }
        public virtual Action<IResponseResult<IApiResponse<Tuple<T, IEnumerable<TUserBlobs>, IEnumerable<TUserBlobs>>>>> OnResponseUpdateWithBlobsException { get; set; }
        public virtual Action<IResponseResult<IApiResponse<Tuple<T, IEnumerable<TUserBlobs>, IEnumerable<TUserBlobs>>>>> OnResponseUpdateWithBlobsFailed { get; set; }
        public virtual Action<Action> OnBeforeReset { get; set; }
        public virtual Action OnAfterReset { get; set; }
        public virtual Action<Action> OnBeforeCancel { get; set; }
        public virtual Action OnAfterCancel { get; set; }
        public virtual Action<Action> OnBeforeSave { get; set; }
        public virtual Action OnAfterSave { get; set; }
        public virtual Action<Action> OnBeforeDelete { get; set; }
        public virtual Action<T> OnAfterDelete { get; set; }
        public virtual async Task Load()
        {
            if(Identifier != null)
            {
                Response = await HttpService.GetOne<T>(Identifier, new EmptyValidator());
                if(Response.Succeed && Response.Response.Status.Succeeded())
                {
                    Record = Response.Response.Data;
                    OnLoadSucceeded?.Invoke(Response);
                }
                else if(Response.HasException)
                {
                    OnLoadException?.Invoke(Response);
                }
                else
                {
                    OnLoadFailed?.Invoke(Response);
                }
            }
            OnLoad?.Invoke(Response);
        }
        public virtual async Task Save(IValidator validator)
        {
            if(FormState == FormState.Copy || FormState == FormState.Create || FormState == FormState.Edit)
            {
                if(FormState == FormState.Edit)
                {
                    if(OnAfterSave == null)
                    {
                        await InternalUpdate(validator);
                    }
                    else
                    {
                        OnBeforeSave.Invoke(async () => await InternalUpdate(validator));
                    }
                }
                else
                {
                    if (OnAfterSave == null)
                    {
                        await InternalCreate(validator);
                    }
                    else
                    {
                        OnBeforeSave.Invoke(async () => await InternalCreate(validator));
                    }
                }
            }
        }
        private async Task InternalCreate(IValidator validator)
        {
            if (BlobDatas.Any())
            {
                ResponseAddWithBlobs = await HttpService.Add(Record, validator, BlobDatas);
                OnResponseAddWithBlobs?.Invoke(ResponseAddWithBlobs);
                if (Response.Succeed && Response.Response.Status.Succeeded())
                {
                    Record = Response.Response.Data;
                    FormState = FormState.Edit;
                    OnResponseAddWithBlobsSucceeded?.Invoke(ResponseAddWithBlobs);
                }
                else if (Response.HasException)
                {
                    OnResponseAddWithBlobsException?.Invoke(ResponseAddWithBlobs);
                }
                else
                {
                    OnResponseAddWithBlobsFailed?.Invoke(ResponseAddWithBlobs);
                }
            }
            else
            {
                Response = await HttpService.Add<T>(Record, validator);
                InternalResponse(FormState.Edit);
            }
            OnAfterSave?.Invoke();
        }
        private async Task InternalUpdate(IValidator validator)
        {
            if (BlobDatas.Any())
            {
                ResponseUpdateWithBlobs = await HttpService.Update(Record, validator, BlobDatas);
                OnResponseUpdateWithBlobs?.Invoke(ResponseUpdateWithBlobs);
                if (Response.Succeed && Response.Response.Status.Succeeded())
                {
                    Record = Response.Response.Data;
                    FormState = FormState.Edit;
                    OnResponseUpdateWithBlobsSucceeded?.Invoke(ResponseUpdateWithBlobs);
                }
                else if (Response.HasException)
                {
                    OnResponseUpdateWithBlobsException?.Invoke(ResponseUpdateWithBlobs);
                }
                else
                {
                    OnResponseUpdateWithBlobsFailed?.Invoke(ResponseUpdateWithBlobs);
                }
            }
            else
            {
                Response = await HttpService.Update<T>(Record, validator);
                InternalResponse(FormState.Edit);
            }
            OnAfterSave?.Invoke();
        }
        private void InternalResponse(FormState formState)
        {
            OnResponse?.Invoke(Response);
            if (Response.Succeed && Response.Response.Status.Succeeded())
            {
                Record = Response.Response.Data;
                FormState = formState;
                OnResponseSucceeded?.Invoke(Response);
            }
            else if (Response.HasException)
            {
                OnResponseException?.Invoke(Response);
            }
            else
            {
                OnResponseFailed?.Invoke(Response);
            }
        }
        public virtual async Task Delete(IValidator validator)
        {
            if (FormState == FormState.Delete)
            {
                if(OnBeforeDelete == null)
                {
                    Response = await HttpService.Delete<T>(Identifier, validator);
                    InternalResponse(FormState.Deleted);
                }
                else
                {
                    OnBeforeDelete.Invoke(async () => { 
                        Response = await HttpService.Delete<T>(Identifier, validator); 
                        InternalResponse(FormState.Deleted); 
                    });
                }
                OnAfterDelete?.Invoke(Response.Response.Data);
            }
        }
        public virtual Task Reset()
        {
            if(OnBeforeReset == null)
            {
                return InternalReset();
            }
            OnBeforeReset.Invoke(() => InternalReset());
            return Task.CompletedTask;
        }
        private Task InternalReset()
        {
            Record = new T();
            Identifier = new TIdentifier();
            BlobDatas.Clear();
            OnAfterReset?.Invoke();
            return Task.CompletedTask;
        }
        public virtual Task Cancel()
        {
            if (OnBeforeCancel == null)
            {
                return InternalCancel();
            }
            OnBeforeCancel.Invoke(() => InternalCancel());
            return Task.CompletedTask;
        }
        private Task InternalCancel()
        {
            Record = new T();
            Identifier = new TIdentifier();
            BlobDatas.Clear();
            OnAfterCancel?.Invoke();
            return Task.CompletedTask;
        }
    }
}