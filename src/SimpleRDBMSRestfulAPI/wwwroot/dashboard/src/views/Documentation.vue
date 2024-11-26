<template>
  <a-layout has-sider>
    <a-layout-sider :style="{ overflow: 'auto', height: '100vh', position: 'fixed', left: 0, top: 0, bottom: 0 }">
      <site-logo></site-logo>
      <!-- TODO: create component Menu.vue -->
      <side-bar :selectedKeys="selectedKeys"></side-bar>
    </a-layout-sider>
    <a-layout :style="{ marginLeft: '200px' }">
      <!-- <a-layout-header style="color: var(--color-light); padding:  0 10px; background-color: var(--bg-color-dark);">
        Header
      </a-layout-header> -->
      <a-layout-content
        :style="{ margin: '24px 16px 0', overflow: 'auto', flex: 1, display: 'flex', flexDirection: 'column', }">
        <table-list></table-list>
        <!-- <a-divider style="margin: 10px 0;" /> -->
      </a-layout-content>
      <!-- TODO: create component Footer.vue -->
      <a-layout-footer :style="{ textAlign: 'center' }">
        Simple RDBMS RESTful API ©2024 - Created by HexaSync Inc.
      </a-layout-footer>
    </a-layout>
  </a-layout>
</template>
<script lang="ts" setup>
import { defineAsyncComponent, ref } from 'vue';
const TableList = defineAsyncComponent(() => import('@/components/tables/TableList.vue'));

const selectedKeys = ref<string[]>(['2']);
</script>
<style scoped></style>
