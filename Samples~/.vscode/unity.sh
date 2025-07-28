#!/bin/bash

# 获取Unity版本号
get_version() {
    local version_file="$1/ProjectSettings/ProjectVersion.txt"
    if [ -f "$version_file" ]; then
        grep "^m_EditorVersion:" "$version_file" | cut -d" " -f2
    else
        echo ""
    fi
}

# 获取Unity可执行文件路径
get_executable() {
    local version="$1"
    local executable=""

    # 检查默认安装路径
    if [ "$(uname)" = "Darwin" ]; then
        default_path="/Applications/Unity/Hub/Editor/$version/Unity.app/Contents/MacOS/Unity"
        if [ -f "$default_path" ]; then
            executable="$default_path"
        fi
    else
        default_path="C:/Program Files/Unity/Hub/Editor/$version/Editor/Unity.exe"
        if [ -f "$default_path" ]; then
            executable="$default_path"
        fi
    fi

    # 如果默认路径不存在，使用环境变量
    if [ -z "$executable" ]; then
        executable="Unity"
    fi

    echo "$executable"
}

# 检查进程是否存在
check_pid() {
    local pid=$1
    if [ "$(uname)" = "MINGW"* ] || [ "$(uname)" = "MSYS"* ]; then
        tasklist 2>/dev/null | grep -q "$pid"
    else
        kill -0 "$pid" 2>/dev/null
    fi
}

# 关闭已存在的Unity实例
close_existing() {
    local pid_file="unity.pid"
    
    if [ -f "$pid_file" ]; then
        local old_pid=$(cat "$pid_file")
        if [ ! -z "$old_pid" ] && check_pid "$old_pid"; then
            echo "Found existing Unity instance (PID: $old_pid), closing..."
            if [ "$(uname)" = "MINGW"* ] || [ "$(uname)" = "MSYS"* ]; then
                taskkill //PID $old_pid //F //T >/dev/null 2>&1
            else
                kill -TERM $old_pid 2>/dev/null
                sleep 1
                if check_pid "$old_pid"; then
                    kill -KILL $old_pid 2>/dev/null
                fi
            fi
        fi
        rm -f "$pid_file"
    fi
}

# 清理函数
cleanup() {
    # 终止Unity进程
    if [ ! -z "$UNITY_PID" ]; then
        if [ "$(uname)" = "MINGW"* ] || [ "$(uname)" = "MSYS"* ]; then
            taskkill //PID $UNITY_PID //F //T >/dev/null 2>&1
        else
            kill -TERM $UNITY_PID 2>/dev/null
            sleep 1
            if check_pid "$UNITY_PID"; then
                kill -KILL $UNITY_PID 2>/dev/null
            fi
        fi
        wait $UNITY_PID 2>/dev/null
        rm -f "unity.pid"
    fi

    # 终止tail进程
    if [ ! -z "$TAIL_PID" ]; then
        kill $TAIL_PID 2>/dev/null
        wait $TAIL_PID 2>/dev/null
    fi
}

# 主函数
main() {
    local project_path="$1"
    shift  # 移除第一个参数（项目路径）

    # 获取版本号
    local version=$(get_version "$project_path")
    if [ -z "$version" ]; then
        echo "Error: Could not find Unity version in ProjectSettings/ProjectVersion.txt" >&2
        exit 1
    fi

    # 获取Unity可执行文件路径
    local executable=$(get_executable "$version")

    # 检查并关闭已存在的Unity实例
    close_existing

    # 创建日志目录
    mkdir -p "$project_path/Library"
    local log_file="$project_path/Library/Editor.log"

    # 设置退出时的清理
    trap cleanup EXIT INT TERM HUP

    # 清空日志文件
    : > "$log_file"

    # 启动tail来监视日志文件
    tail -f "$log_file" &
    TAIL_PID=$!

    # 启动Unity进程并等待其完成
    "$executable" -accept-apiupdate -projectPath "$project_path" -logFile "$log_file" "$@" &
    UNITY_PID=$!

    # 保存PID到文件
    echo $UNITY_PID > "unity.pid"

    # 等待Unity进程
    wait $UNITY_PID
    UNITY_EXIT=$?

    # 终止tail进程并清理
    cleanup

    # 返回Unity的退出码
    exit $UNITY_EXIT
}

# 检查参数
if [ $# -lt 1 ]; then
    echo "Usage: $0 <project-path> [unity-arguments...]" >&2
    exit 1
fi

# 执行主函数
main "$@"