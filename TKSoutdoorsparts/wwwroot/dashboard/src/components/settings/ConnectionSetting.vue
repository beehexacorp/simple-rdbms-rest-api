<template>
    <a-card title="Database Connection" class="database-connection" :bordered="false">
        <!-- Connection Info Section -->
        <template v-if="connectionInfo">
            <a-row>
                <a-col :span="24">
                    <p>
                        <strong>Database Type:</strong> {{ connectionInfo.dbType }}
                    </p>
                    <p>
                        <strong>Database:</strong> {{ connectionInfo.database }}
                    </p>
                    <p>
                        <strong>Host:</strong> {{ connectionInfo.host }}
                    </p>
                    <p>
                        <strong>Port:</strong> {{ connectionInfo.port }}
                    </p>
                    <p>
                        <strong>User:</strong> {{ connectionInfo.user }}
                    </p>
                </a-col>
                <a-col :span="24" style="margin-top: 20px;">
                    <a-button type="primary" @click="testConnectionFromConfigs">Test Connection</a-button>
                </a-col>
            </a-row>
        </template>

        <!-- Connection Form Section -->
        <template v-else>
            <a-form layout="vertical" @submit.prevent="saveConnectionInfo">
                <a-form-item label="Database Type">
                    <a-select v-model:value="selectedDbType" @change="onDbTypeChanged" :options="dbTypes"
                        placeholder="Select Database Type" />
                </a-form-item>
                <a-form-item label="Connection String">
                    <a-input v-model:value.lazy="connectionString" :placeholder="connectionStringPlaceholder" />
                </a-form-item>
                <a-form-item>
                    <a-button type="primary" @click="testConnection">Test Connection</a-button>
                    <a-button style="margin-left: 10px;" @click="saveConnectionInfo">Save</a-button>
                </a-form-item>
            </a-form>
        </template>
    </a-card>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from "vue";
import {
    fetchConnectionInfo,
    fetchDbTypes,
    tryConnect,
    saveConnection,
} from "@/services/connectionService";
import type { ConnectionInfoViewModel } from "@/services/connectionService";
import { useMessage } from '@/utils/message';


const $message = useMessage()

// Reactive data
const connectionInfo = ref<ConnectionInfoViewModel | null>(null);
const dbTypes = ref<{ value: number; label: string }[]>([]);
const selectedDbType = ref<number | null>(4);
const connectionString = ref<string>("Server=shared-postgres.demo.svc.cluster.local;Port=5432;Database=postgres;Userid=postgres;Password=vJ)cPF2ZDYsQIg.N;Pooling=true;MinPoolSize=1;MaxPoolSize=100;Include Error Detail=true");

// Placeholder for connection string
const connectionStringPlaceholder = computed(() => {
    switch (selectedDbType.value) {
        case 0: // SQL_ANYWHERE
            return "e.g., Anywhere:ENG=server_name;DBN=database_name;UID=user;PWD=password;";
        case 1: // SQL_SERVER
            return "e.g., Server=server_name;Database=database_name;User Id=user;Password=password;";
        case 2: // ORACLE
            return "e.g., (DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=host)(PORT=port))(CONNECT_DATA=(SERVICE_NAME=service_name)))";
        case 3: // MYSQL
            return "e.g., mysql://user:password@host:port/database";
        case 4: // POSTGRES
            return "e.g., postgresql://user:password@host:port/database";
        default:
            return "Enter connection string...";
    }
});

// // Watcher for database type changes (optional)
// watch(selectedDbType, (newType, oldType) => {
//     // Reset connection string when database type changes
//     connectionString.value = "";
// });

const onDbTypeChanged = (v: number) => {
    console.log(`Database type changed from ${selectedDbType.value} to ${v}`);
    selectedDbType.value = v;
    connectionString.value = "";
}

// Load connection info and database types
const loadConnectionSettings = async () => {
    try {
        dbTypes.value = await fetchDbTypes();
        selectedDbType.value = 4;
        connectionInfo.value = await fetchConnectionInfo();
    } catch (error) {
        $message('error', `Error loading connection settings: ${error}`, error);
    }
};

// Test connection from configs
const testConnectionFromConfigs = async () => {
    try {
        await tryConnect({ useConfig: true });
        $message('success', "Connection successful!");
    } catch (error) {
        $message('error', `Connection test failed: ${error}`, error);
    }
};

// Test connection with user input
const testConnection = async () => {
    try {
        if (!selectedDbType.value || !connectionString.value) {
            throw new Error("Database type and connection string are required.");
        }
        await tryConnect({
            dbType: selectedDbType.value,
            connectionString: connectionString.value,
        });
        $message('success', "Connection successful!");
    } catch (error) {
        $message('error', `Connection test failed: ${error}`, error);
    }
};

// Save connection
const saveConnectionInfo = async () => {
    try {
        if (!selectedDbType.value || !connectionString.value) {
            throw new Error("Database type and connection string are required.");
        }
        await saveConnection({
            dbType: selectedDbType.value,
            connectionString: connectionString.value,
        });
        $message('success', "Connection saved!");
    } catch (error) {
        $message('error', `Failed to save connection: ${error}`, error);
    }
};

// Lifecycle hooks
onMounted(loadConnectionSettings);
</script>

<style scoped>
.database-connection {}
</style>
