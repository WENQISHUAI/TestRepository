<template> 
        <div>
                <h1>This is login page</h1> 
        <el-row type="flex" justify="center">
            <el-form ref="loginForm" v-if="!isLogin" :model="user" :rules="rules" status-icon label-width="150px">
                <el-form-item label="账号" prop="name">
                    <el-input v-model="user.name"></el-input>
                </el-form-item>
                <el-form-item label="密码" prop="pass">
                    <el-input v-model="user.pass" type="password"></el-input>
                </el-form-item>
                <el-checkbox v-model="remember">记住我</el-checkbox>
                <el-form-item>
                    <el-button type="primary"    icon="el-icon-upload" @click="login">登录</el-button>
                  
                </el-form-item>
            </el-form> 
            <el-button type="primary"   v-else  icon="el-icon-upload" @click="logout">退出登录</el-button>
        </el-row>
    </div>
    </template>

<script>
    const Base64=require('js-base64').Base64;
    export default {
        methods: {
            login() { //使用elementui validate验证
                let _this = this;
                _this.$store.commit("saveToken", "");
                this.$refs.loginForm.validate(valid => {
                    if (valid) {
                        this.$api.get("Values/GetJWTStr", {
                            name: _this.user.name,
                            pass: _this.user.pass
                        }, res => {
                            if (res.success) {
                                var token = res.token;

                                var curTime = new Date();
                                var expiredate=new Date(curTime.setSeconds(curTime.getSeconds()+res.expires_in));//过期时间
                                _this.$store.commit("saveTokenExpire",expiredate);//保存过期时间
                                _this.$store.commit("saveToken", token);
                                console.log("token",_this.$store.state.token); //打印token，查看是否已存入store
                                console.log("expiredate", new Date(_this.$store.state.expiredate));
                                console.log("refreshTime",new Date(window.localStorage.refreshTime));
                                this.setUserInfo();
                                this.$notify({
                                    type: "success",
                                    message: "欢迎你," + this.user.name + "!",
                                    duration: 3000
                                });
                                if(this.$route.query.redirect){
                                    this.$router.replace(this.$route.query.redirect);
                                }
                                else{
                                    this.$router.replace("/");
                                }  
                            } else {
                                this.$message({
                                    type: "error",
                                    message: "用户名或密码错误",
                                    showClose: true
                                });
                            }
                        })
                    } else {
                        return false;
                    }
                }) 
            },
            logout() {
                this.isLogin = false;
                this.$store.commit("saveToken", ""); //清掉 token
            },
            setUserInfo(){
                if(this.remember){
                    this.setCookie("account",this.user.name);
                    this.setCookie("password",Base64.encode (this.user.pass));
                }
                else{
                    this.setCookie("account","");
                    this.setCookie("password","");
                } 
            },
            getCookie(key){
                if(document.cookie.length>0){
                    var start=document.cookie.indexOf(key+"=");     
                    if(start!=-1){
                        start=start+key.length+1;
                        var end =document.cookie.indexOf(";",start);
                        if(end===-1){
                            end=document.cookie.length;
                        }
                        return unescape(document.cookie.substring(start,end))
                    }
                }
                return "";
            },
            setCookie(cname,value){
                var exdate=new Date(); 
                exdate.setSeconds(exdate.getSeconds()+10);
                document.cookie=cname+"="+escape(value)+";expires="+exdate.toGMTString();
            }
        },
        data() {
            return {
                isLogin: false,
                remember:false,
                user: {}, //配合页面内的 prop 定义数据
                rules: { //配合页面内的 prop 定义规则
                    name: [{
                        required: true,
                        message: "用户名不能为空",
                        trigger: "blur"
                    }],
                    pass: [{
                        required: true,
                        message: "密码不能为空",
                        trigger: "blur"
                    }]
                }
            };
        },
        created() {
            let account = this.getCookie("account")
            let password = Base64.decode(this.getCookie("password"))
            // 如果存在赋值给表单，并且将记住密码勾选
            if(account){
                this.user.name = account
                this.user.pass= password 
                this.remember=true;
            }
            
            if (window.localStorage.token && window.localStorage.token.length >= 128) {
                this.isLogin = true;
            } 
        }
    };
</script>