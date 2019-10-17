import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
    state: {
        token: "",
        expiredate:{}
    },
    mutations: {
        saveToken(state, data) {
            state.token = data; //token
            window.localStorage.setItem("token", data);
        },
        saveTokenExpire(state,data){
            state.expiredate=data; // token过期时间
            window.localStorage.setItem("tokenExpire",data);// 保存刷新时间，这里的和过期时间一致
            window.localStorage.setItem("refreshTime",data);// 保存刷新时间，这里的和过期时间一致
        }
    },
    actions: {

    }
})