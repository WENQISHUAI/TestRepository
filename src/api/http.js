import store from "../store";
import router from "../router.js";
import Vue from 'vue'
//api接口地址
var root = "/api/"

//引用axios
var axios = require('axios')

//自定义判断元素类型
function toType(obj) {
    return ({}).toString.call(obj).match(/\s([a-zA-Z]+)/)[1].toLowerCase()
}

// 参数过滤函数
function filterNull(o) {
    for (var key in o) {
        if (o[key] === null) {
            delete o[key]
        }
        if (toType(o[key]) === 'string') {
            o[key] = o[key].trim()
        } else if (toType(o[key]) === 'object') {
            o[key] = filterNull(o[key])
        } else if (toType(o[key]) === 'array') {
            o[key] = filterNull(o[key])
        }
    }
    return o
}

function ToLogin() {
    store.commit("saveToken", "");
    router.replace({
        path: "login",
        query: { redirect: router.currentRoute.fullPath }
    });
}

// http request 拦截器
axios.interceptors.request.use(
    config => {
        saveRefreshTime();
        if (window.localStorage.token && window.localStorage.token.length >= 128) {
            // 判断是否存在token，如果存在的话，则每个http header都加上token
            config.headers.Authorization = "Bearer " + window.localStorage.token;
        }
        return config;
    },
    err => {
        return Promise.reject(err);
    }
);


// http response 拦截器
axios.interceptors.response.use(
    response => {
        return response;
    },
    error => {
        if (error.response) {
            switch (error.response.status) {
                case 401:
                    var curTime = new Date()
                    var refreshTime = new Date(Date.parse(window.localStorage.refreshTime))
                        // 在用户操作的活跃期内
                    if (window.localStorage.refreshTime && (curTime <= refreshTime)) {
                        // 直接将整个请求 return 出去，不然的话，请求会晚于当前请求，无法达到刷新操作  
                        return refreshToken({ token: window.localStorage.token }, (res) => {
                            if (res.success) {
                                Vue.prototype.$message({
                                    message: 'refreshToken success! loading data...',
                                    type: 'success'
                                });

                                store.commit("saveToken", res.token);

                                var curTime = new Date();
                                var expiredate = new Date(curTime.setSeconds(curTime.getSeconds() + res.expires_in));
                                store.commit("saveTokenExpire", expiredate); 
                                error.config.__isRetryRequest = true;
                                error.config.headers.Authorization = 'Bearer ' + res.token;
                                error.config.baseURL = "";
                                // error.config 包含了当前请求的所有信息
                                return axios(error.config);
                            } else {
                                // 刷新token失败 清除token信息并跳转到登录页面
                                ToLogin()
                            }
                        });
                    } else {
                        // 返回 401，并且不知用户操作活跃期内 清除token信息并跳转到登录页面
                        ToLogin()
                    }
                    break;
                case 403:
                    Vue.prototype.$message({
                        message: '失败！该操作无权限',
                        type: 'error'
                    });
                    return null;
            }
            return Promise.reject(error.response.data); // 返回接口返回的错误信息
        }
    }
);

function refreshToken(params, success) {
    return apiAxios('GET', "Values/RefreshToken", params, success);
}

function apiAxios(method, url, params, success, faliure) {
    if (params) {
        params = filterNull(params)
    }

    axios({
            method: method,
            url: url,
            data: method === "POST" || method === "PUT" ? params : null,
            params: method === "GET" || method === "DELETE" ? params : null,
            //自定义请求头，验证
            // header: { "Authorization": "Bearer token" },
            baseURL: root,
            withCredentials: false
        })
        .then(function(res) {
            if (res.data.success) {
                if (success) {
                    success(res.data)
                }
            } else {
                if (faliure) {
                    faliure(res.data)
                } else {
                    window.alert("error:" + JSON.stringify(res.data))
                }
            }
        })
        .catch(function(err) {
            let res = err.response; 
            if (res) {
                window.alert("api error ,http code:" + res.status);
            }
        })
}
// 返回在vue模板中的调用接口
export default {
    get: function(url, params, success, failure) {
        return apiAxios('GET', url, params, success, failure)
    },
    post: function(url, params, success, failure) {
        return apiAxios('POST', url, params, success, failure)
    },
    put: function(url, params, success, failure) {
        return apiAxios('PUT', url, params, success, failure)
    },
    delete: function(url, params, success, failure) {
        return apiAxios('DELETE', url, params, success, failure)
    }
}

export const saveRefreshTime = params => {

    let nowtime = new Date();
    let lastRefreshTime = window.localStorage.refreshTime ? new Date(window.localStorage.refreshTime) : new Date(-1);
    let expiretime = new Date(Date.parse(window.localStorage.tokenExpire))

    let refreshCount = 1; //滑动系数
    if (lastRefreshTime >= nowtime) {
        lastRefreshTime = nowtime > expiretime ? nowtime : expiretime;
        lastRefreshTime.setMinutes(lastRefreshTime.getMinutes() + refreshCount);
        window.localStorage.refreshTime = lastRefreshTime;
    } else {
        window.localStorage.refreshTime = new Date(-1);
    }
};