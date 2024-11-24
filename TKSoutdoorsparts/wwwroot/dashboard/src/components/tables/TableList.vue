<template>
  <a-card title="Table List" class="table-list" :bordered="false">
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
      <a-button :disabled="!lastCursor" @click="fetchNextPage"> Next </a-button>
    </div>
  </a-card>
</template>

<script setup lang="ts">
import { ref, onMounted, h } from 'vue'
// import { getTables, CursorBasedResult } from "@/services/connectionService";
import { useMessage } from '@/utils/message'
import { debounce } from 'lodash'
import { Button } from 'ant-design-vue'
import { getTables } from '@/services/entityService'
import type { CursorBasedResult } from '@/utils/CursorBasedResult'

const $message = useMessage()

// State Variables
const query = ref<string | null>(null)
const tables = ref<Array<Record<string, unknown>>>([])
const firstCursor = ref<string | null>(null)
const lastCursor = ref<string | null>(null)
const loading = ref(false)
const rel = ref(1) // Cursor direction: 1 for next, 0 for previous
const limit = 10 // Number of items per page

// TODO: Table Columns Definition should be dynamically based on result.items
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
      // Encode the record as a Base64 string
      const base64String = btoa(JSON.stringify(tables.value[record.index as number]))

      // Create the full URL
      const detailUrl = `/dashboard/documentation/entity/detail?detail=${base64String}`

      // Render the Ant Design Link Button
      return h(
        Button,
        {
          type: 'link',
          onClick: () => {
            window.location.href = detailUrl // Navigate to the URL
          },
        },
        {
          default: () => 'View Details', // Button content
        },
      )
    },
  },
]

// Fetch Tables Function
const fetchTables = async () => {
  loading.value = true
  try {
    const result: CursorBasedResult = await getTables(
      query.value,
      rel.value,
      rel.value === 1 ? lastCursor.value : firstCursor.value,
      limit,
    )
    if (!!result?.items?.length) {
      tables.value = result.items
      firstCursor.value = result.firstCursor
      lastCursor.value = result.lastCursor
    }
  } catch (error) {
    $message('error', `Error fetching tables: ${error}`)
  } finally {
    loading.value = false
  }
}

// View Details Function
const viewDetails = ({ index }: { index: number }) => {
  console.log(`Viewing details for table`, tables.value[index])
  // Implement the logic to show details here, e.g., navigating to another page or opening a modal
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

const onSearchTables = debounce(async () => {
  rel.value = 1
  lastCursor.value = null
  firstCursor.value = null
  await fetchTables()
}, 500)

// Fetch Tables on Mount
onMounted(fetchTables)
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
