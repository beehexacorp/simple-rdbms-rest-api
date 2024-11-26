<template>
  <a-card title="Connections Settings" :bordered="false">
    <a-button type="primary" @click="showAddConnectionModal">Add Connection</a-button>
    <a-divider></a-divider>
    <a-list :dataSource="connections" bordered>
      <template #renderItem="{ item }">
        <connection-item :connection="item" />
      </template>
    </a-list>

    <a-modal
      v-model:open="isAddConnectionModalVisible"
      title="Add Connection"
      @ok="handleAddConnection"
      @cancel="closeAddConnectionModal"
    >
      <a-form ref="addConnectionForm" layout="vertical" :model="newConnection">
        <a-form-item
          label="Database Type"
          name="dbType"
          :rules="[{ required: true, message: 'Please select a database type' }]"
        >
          <a-select
            v-model:value="newConnection.dbType"
            placeholder="Select Database Type"
            @change="updateDefaultConnectionString"
          >
            <a-select-option v-for="dbType in dbTypes" :key="dbType.value" :value="dbType.value">
              {{ dbType.label }}
            </a-select-option>
          </a-select>
        </a-form-item>
        <a-form-item
          label="Connection String"
          name="connectionString"
          :rules="[{ required: true, message: 'Connection string is required' }]"
        >
          <a-input
            v-model:value="newConnection.connectionString"
            :placeholder="connectionStringPlaceholder"
          />
        </a-form-item>
      </a-form>
    </a-modal>
  </a-card>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { fetchConnectionInfos, fetchDbTypes, saveConnection } from '@/services/connectionService'
import type { SaveConnectionRequest } from '@/services/connectionService'
import type { Connection } from '@/types/Connection'
import type { DbType } from '@/types/DbType'

const connections = ref<Connection[]>([])
const dbTypes = ref<DbType[]>([])
const isAddConnectionModalVisible = ref(false)
const addConnectionForm = ref()
const connectionStringPlaceholder = ref('')
const newConnection = ref<SaveConnectionRequest>({
  dbType: 4,
  connectionString: '',
})

// Define default placeholders for different database types
const defaultConnectionStrings: Record<number, string> = {
  0: 'Driver={SQL Anywhere};Server=servername;Database=dbname;Uid=username;Pwd=password;', // SQL Anywhere (ODBC)
  1: 'Driver={Oracle};Dbq=//host:port/servicename;Uid=username;Pwd=password;', // Oracle (ODBC)
  2: 'Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;', // SQL Server
  3: 'Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;', // MySQL
  4: 'Server=myServer;Port=5432;Database=mydb;Userid=myuser;Password=mypassword;', // Postgres (Npgsql)
}

const updateDefaultConnectionString = () => {
  connectionStringPlaceholder.value =
    defaultConnectionStrings[newConnection.value.dbType] || 'Enter your connection string'
}

const loadConnections = async () => {
  try {
    const result = await fetchConnectionInfos()
    console.log(result)
    connections.value = result || []
  } catch (error: any) {
    console.error('Failed to load connections:', error.message)
  }
}

const loadDbTypes = async () => {
  try {
    dbTypes.value = await fetchDbTypes()
  } catch (error: any) {
    console.error('Failed to load database types:', error.message)
  }
}

const showAddConnectionModal = () => {
  isAddConnectionModalVisible.value = true
  updateDefaultConnectionString()
}

const closeAddConnectionModal = () => {
  isAddConnectionModalVisible.value = false
  addConnectionForm.value?.resetFields()
}

const handleAddConnection = async () => {
  try {
    await addConnectionForm.value?.validate()
    await saveConnection(newConnection.value!)
    await loadConnections()
    closeAddConnectionModal()
  } catch (error: any) {
    console.error('Failed to add connection:', error.message)
  }
}

onMounted(async () => {
  await loadConnections()
  await loadDbTypes()
})
</script>
