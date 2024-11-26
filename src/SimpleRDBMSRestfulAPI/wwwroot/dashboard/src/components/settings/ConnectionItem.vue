<template>
  <a-list-item>
    <a-list-item-meta
      :title="`Database: ${connection.database}`"
      :description="`Host: ${connection.host}, Port: ${connection.port}, User: ${connection.user}`"
    />
    <template #actions>
      <a-button type="primary" @click="handleTestConnection">Test Connection</a-button>
    </template>
  </a-list-item>
</template>

<script setup lang="ts">
import { tryConnect } from '@/services/connectionService'
import type { Connection } from '@/types/Connection'
import { useMessage } from '@/utils/message'

const $message = useMessage()
const props = defineProps({
  connection: {
    type: Object as () => Connection,
    required: true,
  },
})

const handleTestConnection = async () => {
  try {
    console.log(props.connection)
    await tryConnect({ connectonId: props.connection?.id })
    $message.success('Connection successful!')
  } catch (error: any) {
    $message.error(`Connection failed: ${error}`)
  }
}
</script>
