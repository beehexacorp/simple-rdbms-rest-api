import { message } from 'ant-design-vue'

export function useMessage() {
  return {
    error: (content: string) => {
      message['error']({
        content,
        duration: 10,
      })
    },
    success: (content: string) => {
      message['success']({
        content,
        duration: 10,
      })
    },
    info: (content: string) => {
      message['info']({
        content,
        duration: 10,
      })
    },
  }
}
