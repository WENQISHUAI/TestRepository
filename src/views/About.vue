<template>
  <div class="about">
    <h1>This is an search page</h1> 
    <el-input v-model="dataId" ></el-input>
    <el-button type="primary"  @click="search">查询</el-button>
    {{gvalues}}
  </div>
</template>
<script>
    export default {
        name: "about", 
        data() {
            return {
                dataId:0,
                gvalues: {}
            }
        },
        methods: {
            getData() {
                var _this = this;
                this.$api.get("Values/GetData", {
                    id: _this.dataId
                }, r => {
                    _this.gvalues = r;
                })
            },
            search(){
                this.getData();
            }
        },
    }
</script>