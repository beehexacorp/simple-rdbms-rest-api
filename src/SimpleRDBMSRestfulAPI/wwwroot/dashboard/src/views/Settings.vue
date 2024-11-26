<template>
    <a-layout has-sider>
        <a-layout-sider :style="{ overflow: 'auto', height: '100vh', position: 'fixed', left: 0, top: 0, bottom: 0 }">
            <site-logo></site-logo>
            <side-bar :selectedKeys="selectedKeys"></side-bar>
        </a-layout-sider>
        <a-layout :style="{ marginLeft: '200px' }">
            <a-layout-content
                :style="{ margin: '24px 16px 0', overflow: 'auto', flex: 1, display: 'flex', flexDirection: 'column', }">
                <connection-setting />
                <!-- <a-divider style="margin: 10px 0;" /> -->
            </a-layout-content>
            <a-layout-footer :style="{ textAlign: 'center' }">
                Simple RDBMS RESTful API ©2024 - Created by HexaSync Inc.
            </a-layout-footer>
        </a-layout>
    </a-layout>
</template>
<script lang="ts" setup>
import { defineAsyncComponent, ref } from 'vue';

const selectedKeys = ref<string[]>(['3']);
</script>
<style scoped></style>