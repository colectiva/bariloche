using La_Colectiva.Models.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.Services.Api
{
    public interface ApiService
    {


        ApiRequest<R, T> AbsDelete<R, T>(string url);
        ApiRequest<R, R> AbsDelete<R>(string url);
        ApiRequest<R, T> AbsGet<R, T>(string url);
        ApiRequest<R, R> AbsGet<R>(string url);
        ApiRequest<R, T> AbsHead<R, T>(string url);
        ApiRequest<R, R> AbsHead<R>(string url);
        ApiRequest<R, T> AbsOptions<R, T>(string url);
        ApiRequest<R, R> AbsOptions<R>(string url);
        ApiRequest<R, T> AbsPost<R, T>(string url);
        ApiRequest<R, R> AbsPost<R>(string url);
        ApiRequest<R, T> AbsPut<R, T>(string url);
        ApiRequest<R, R> AbsPut<R>(string url);
        ApiRequest<R, T> AbsRequest<R, T>(string url, HttpMethod method);
        ApiRequest<R, R> AbsRequest<R>(string url, HttpMethod method);

        ApiRequest<R, T> Delete<R, T>(string relativeUrl);
        ApiRequest<R, R> Delete<R>(string relativeUrl);
        ApiRequest<R, T> Get<R, T>(string relativeUrl);
        ApiRequest<R, R> Get<R>(string relativeUrl);
        ApiRequest<R, T> Head<R, T>(string relativeUrl);
        ApiRequest<R, R> Head<R>(string relativeUrl);
        ApiRequest<R, T> Options<R, T>(string relativeUrl);
        ApiRequest<R, R> Options<R>(string relativeUrl);
        ApiRequest<R, T> Post<R, T>(string relativeUrl);
        ApiRequest<R, R> Post<R>(string relativeUrl);
        ApiRequest<R, T> Put<R, T>(string relativeUrl);
        ApiRequest<R, R> Put<R>(string relativeUrl);
        ApiRequest<R, T> Request<R, T>(string relativeUrl, HttpMethod method);
        ApiRequest<R, R> Request<R>(string relativeUrl, HttpMethod method);
    }
}
