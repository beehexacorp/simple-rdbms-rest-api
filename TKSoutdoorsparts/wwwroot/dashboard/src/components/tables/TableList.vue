<template>
  <a-card title="Table List" class="table-list" :bordered="false">
    <!-- Connection Selector -->
    <div style="margin-bottom: 16px">
      <a-select
        v-model:value="selectedConnectionId"
        placeholder="Select Connection"
        style="width: 300px"
        @change="onConnectionChange"
      >
        <a-select-option
          v-for="connection in connections"
          :key="connection.id"
          :value="connection.id"
        >
          {{ connection.database }} - {{ connection.host }}
        </a-select-option>
      </a-select>
    </div>

    <!-- Table Search -->
    <a-input-search
      v-model:value="query"
      placeholder="Search for tables..."
      @input="onSearchTables"
      style="width: 300px; margin-bottom: 16px"
    />

    <!-- Table -->
    <a-table
      :columns="columns"
      :data-source="tables"
      :pagination="false"
      row-key="table_name"
      :loading="loading"
    />

    <!-- Pagination Controls -->
    <div class="pagination-controls">
      <a-button :disabled="!firstCursor" @click="fetchPreviousPage" style="margin-right: 8px">
        Previous
      </a-button>
      <a-button :disabled="!lastCursor" @click="fetchNextPage">Next</a-button>
    </div>
  </a-card>
</template>

<script setup lang="ts">
import { ref, onMounted, h } from 'vue'
import { useMessage } from '@/utils/message'
import { debounce } from 'lodash'
import { Button } from 'ant-design-vue'
import { fetchConnectionInfos } from '@/services/connectionService'
import { getTables } from '@/services/entityService'
import type { Connection } from '@/types/Connection'
import type { CursorBasedResult } from '@/utils/CursorBasedResult'

const $message = useMessage()

// State Variables
const connections = ref<Connection[]>([])
const selectedConnectionId = ref<string | null>(null)
const query = ref<string | null>(null)
const tables = ref<Array<Record<string, unknown>>>([])
const firstCursor = ref<string | null>(null)
const lastCursor = ref<string | null>(null)
const loading = ref(false)
const rel = ref(1) // Cursor direction: 1 for next, 0 for previous
const limit = 10 // Number of items per page

// Table Columns Definition
const columns = [
  {
    title: 'Table Catalog',
    dataIndex: 'table_catalog',
    key: 'table_catalog',
  },
  {
    title: 'Table Schema',
    dataIndex: 'table_schema',
    key: 'table_schema',
  },
  {
    title: 'Table Name',
    dataIndex: 'table_name',
    key: 'table_name',
  },
  {
    title: 'Actions',
    key: 'actions',
    align: 'center',
    customRender: (record: Record<string, unknown>) => {
      const base64String = btoa(JSON.stringify(tables.value[record.index as number]))
      const detailUrl = `/dashboard/documentation/entity/detail?connectionId=${selectedConnectionId.value}&detail=${base64String}`

      return h(
        Button,
        {
          type: 'link',
          onClick: () => {
            window.location.href = detailUrl
          },
        },
        {
          default: () => 'View Details',
        },
      )
    },
  },
]

// Fetch Connections
const loadConnections = async () => {
  try {
    const result = await fetchConnectionInfos()
    if (result) {
      connections.value = result
      if (connections.value.length > 0) {
        selectedConnectionId.value = connections.value[0].id // Default to the first connection
        await fetchTables() // Load tables for the default connection
      }
    }
  } catch (error) {
    $message.error(`Error fetching connections: ${error}`)
  }
}

// Fetch Tables Function
const fetchTables = async () => {
  if (!selectedConnectionId.value) return

  loading.value = true
  try {
    const result: CursorBasedResult = await getTables(
      selectedConnectionId.value,
      query.value,
      rel.value,
      rel.value === 1 ? lastCursor.value : firstCursor.value,
      limit,
    )
    if (result?.items?.length) {
      tables.value = result.items
      firstCursor.value = result.firstCursor
      lastCursor.value = result.lastCursor
    } else {
      tables.value = []
      firstCursor.value = null
      lastCursor.value = null
    }
  } catch (error) {
    $message.error(`Error fetching tables: ${error}`)
  } finally {
    loading.value = false
  }
}

// Fetch Next Page
const fetchNextPage = () => {
  rel.value = 1
  fetchTables()
}

// Fetch Previous Page
const fetchPreviousPage = () => {
  rel.value = 0
  fetchTables()
}

// Handle Connection Change
const onConnectionChange = async () => {
  query.value = null
  firstCursor.value = null
  lastCursor.value = null
  await fetchTables()
}

// Handle Table Search
const onSearchTables = debounce(async () => {
  rel.value = 1
  firstCursor.value = null
  lastCursor.value = null
  await fetchTables()
}, 500)

// Load Connections on Mount
onMounted(loadConnections)
</script>

<style scoped>
.table-list {
  padding: 16px;
}

.pagination-controls {
  margin-top: 16px;
  text-align: right;
}
</style>
