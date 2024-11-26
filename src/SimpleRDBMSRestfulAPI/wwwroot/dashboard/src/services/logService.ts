import msgpack from 'msgpack-lite'
import { useServiceEndpoint } from '@/utils/serviceEndpoint'
import { log } from 'console'

export interface LogEntry {
  filename: string
  displayName: string
}

const serviceEndpointHandler = useServiceEndpoint()

/**
 * Fetch history logs for a specific date (timestamp in milliseconds).
 */
export const fetchHistoryLogs = async (timestamp: number): Promise<LogEntry[]> => {
  const apiUrl = serviceEndpointHandler.normalize(`api/history/logs?ts=${timestamp}`)
  const response = await fetch(apiUrl, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
  })

  if (!response.ok) {
    const { errorMessage } = await response.json()
    throw new Error(errorMessage)
  }

  const data: { filename: string }[] = await response.json()

  return data.map((log) => {
    const match = log.filename.match(/^log(\d{4})(\d{2})(\d{2})(\d{2})\.txt$/)
    if (match) {
      const [, year, month, day, hour] = match
      const logDate = `${year}-${month}-${day}T${hour}:00:00`
      return {
        filename: log.filename,
        displayName: new Date(logDate).toLocaleString('en-US', {
          month: 'short',
          day: 'numeric',
          year: 'numeric',
          hour: 'numeric',
          minute: '2-digit',
        }),
      }
    }
    return { filename: log.filename, displayName: log.filename }
  })
}

/**
 * Fetch log detail for a specific file.
 */
export const fetchLogDetail = async (filename: string): Promise<string> => {
  const apiUrl = serviceEndpointHandler.normalize(
    `api/history/log-detail?fn=${encodeURIComponent(filename)}`,
  )
  const response = await fetch(apiUrl, {
    headers: {
      Accept: 'application/x-msgpack',
    },
  })

  if (!response.ok) {
    const { errorMessage } = await response.json()
    throw new Error(errorMessage)
  }

  const buffer = await response.arrayBuffer()
  const decodedContent = msgpack.decode(new Uint8Array(buffer))

  return decodedContent as string
}

/**
 * Download a log file by filename.
 */
export const downloadLogFile = async (filename: string): Promise<void> => {
  const apiUrl = serviceEndpointHandler.normalize(
    `api/history/download?fn=${encodeURIComponent(filename)}`,
  )
  const response = await fetch(apiUrl)

  if (!response.ok) {
    const { errorMessage } = await response.json()
    throw new Error(errorMessage)
  }

  const blob = await response.blob()
  const url = window.URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = filename
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}
