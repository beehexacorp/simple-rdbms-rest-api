import { message } from 'ant-design-vue'

export function useMessage() {
  return async (logLevel: 'success' | 'error', content: string, exception?: any) => {
    message[logLevel]({
      content,
      duration: 10,
    })
  }
}
